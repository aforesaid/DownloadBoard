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
        public static List<Webinar> Items { get; set; } = new();
        private static int countInWork = 0;
        private readonly HttpClient _httpClient = new();
        private int _countThread = 20;
        public MireaApiClient(string cookie)
        {
            _httpClient.DefaultRequestHeaders.Add("Cookie",cookie);
        }

        public async Task StartFind(int firstId, int lastId)
        {
            using var semaphoreSlim = new SemaphoreSlim(10);
            var tasks = Enumerable.Range(firstId, lastId)
                .Select(async x =>
                {
                    try
                    {
                        await semaphoreSlim.WaitAsync();
                        await GetInfoByOnec(x);
                    }
                    finally
                    {
                        semaphoreSlim.Release();
                    }
                });
           
            await Task.WhenAll(tasks);
        }

        private async Task GetInfoByOnec(int id)
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

            var result = await _httpClient.PostAsync(new Uri(url), content);
            var resultInString = await result.Content.ReadAsStringAsync();

            var doc = new HtmlDocument();
            doc.LoadHtml(resultInString);
            var nodes = doc.DocumentNode.SelectNodes("//tr[@class='']");
            if (nodes == null)
                return;
            foreach (var node in nodes)
            {
                try
                {
                    if (node.InnerText.Length < 50)
                        continue;
                
                    var nodeItems = node.InnerText.Split('\n').Skip(1).ToArray();
                    var linkValue = node.SelectNodes("//td[@class='cell c5 lastcol']").First().ChildNodes.First().Attributes["href"].Value;
                    var downloadItem = new Webinar(
                    nodeItems[0],
                    nodeItems[1],
                    nodeItems[2],
                    nodeItems[3],
                    nodeItems[4],
                    linkValue);
                    Items.Add(downloadItem);
                    if (Items.Count % 50 == 0)
                        Console.WriteLine(Items.Count);
                
                }
                catch (Exception e)
                {
                }
            }
            
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}