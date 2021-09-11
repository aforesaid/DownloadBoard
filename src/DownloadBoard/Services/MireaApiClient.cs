using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using DownloadBoard.Models;
using HtmlAgilityPack;

namespace DownloadBoard.Services
{
    public class MireaApiClient : IDisposable
    {
        private static int countInWork = 0;
        private readonly HttpClient _httpClient = new();
        private int _countThread = 20;
        public MireaApiClient(string cookie)
        {
            _httpClient.DefaultRequestHeaders.Add("Cookie",cookie);
        }
        public async Task<Webinar[]> GetWebinarsFromApi(bool sort)
        {
                        
            using var semaphoreSlim = new SemaphoreSlim(10);

            const int startIndex = 1;
            const int lastIndex = 100000;

            var result = new List<Webinar>();
            
            var tasks = Enumerable.Range(startIndex, lastIndex)
                .Select(async x =>
                {
                    try
                    {
                        await semaphoreSlim.WaitAsync();
                        var newItems = await GetInfoByOnec(x);
                        result.AddRange(newItems);
                    }
                    finally
                    {
                        semaphoreSlim.Release();
                    }
                });
           
            await Task.WhenAll(tasks);
            
           
            if (sort)
            {
                result = result.OrderBy(x => x.DateStart)
                    .ToList();
            }
            
            return result.ToArray();
        }

        private async Task<Webinar[]> GetInfoByOnec(int id)
        {
            try
            {
                var url = "https://online-edu.mirea.ru/mod/webinars/studentTable.php";
                var contentInDictionary = new Dictionary<string, string>
                {
                    ["idelement"] = id.ToString()
                };

                var content = new FormUrlEncodedContent(contentInDictionary);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                var response = await _httpClient.PostAsync(new Uri(url), content);
                var resultInString = await response.Content.ReadAsStringAsync();

                var doc = new HtmlDocument();
                doc.LoadHtml(resultInString);
                
                var nodes = doc.DocumentNode.SelectNodes("//tr[@class='']");
                
                return nodes == null ? Array.Empty<Webinar>() : 
                    (
                        from node in nodes.Where(x => x.InnerText.Length >= 50) 
                        let nodeItems = node.InnerText.Split('\n')
                            .Skip(1)
                            .ToArray()
                        let linkValue = node.SelectNodes("//td[@class='cell c5 lastcol']")
                            .First().ChildNodes
                            .First().Attributes["href"].Value
                        select new Webinar(nodeItems[0], 
                            nodeItems[1], 
                            nodeItems[2],
                            nodeItems[3], 
                            nodeItems[4],
                            linkValue)
                        )
                    .ToArray();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}