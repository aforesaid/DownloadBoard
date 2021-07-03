using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace DownloadBoard
{
    public class HttpWorker
    {
        public static List<DownloadItem> Items { get; set; } = new List<DownloadItem>();
        private static int countInWork = 0;
        private int _countThread = 20;
        private string Cookie { get;  }
        public HttpWorker(string cookie)
        {
            Cookie = cookie;
        }

        public void StartFind(int firstId, int lastId)
        {
            var tasks = new List<Task>();
            for (int i = 0; i < _countThread; i++)
            {
                var i1 = i;
                var newTask = Task.Run(ParallelTaskWorker(i1, lastId, _countThread).Wait);
                tasks.Add(newTask);
            }
            Task.WaitAll(tasks.ToArray());
        }

        private async Task ParallelTaskWorker(int startId, int lastId, int countThread)
        {
            for (int i = startId; i < lastId; i+= countThread)
            {
                try
                {

                 await GetInfoByOnec(i);
                 countInWork++;
                 if (countInWork % 50 == 0)
                 {
                     Console.WriteLine(countInWork);
                 }
                 
                }
                catch (Exception e)
                {
                }
            }
        }

        private async Task GetInfoByOnec(int id)
        {
            using var httpClient = new HttpClient();
            var url = "https://online-edu.mirea.ru/mod/webinars/studentTable.php";
            httpClient.DefaultRequestHeaders.Add("Cookie",Cookie);
            var contentInDictionary = new Dictionary<string, string>
            {
                ["idelement"] = id.ToString()
            };

            var content = new FormUrlEncodedContent(contentInDictionary);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var result = await httpClient.PostAsync(new Uri(url), content);
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
                    var downloadItem = new DownloadItem(
                    nodeItems[0],
                    nodeItems[1],
                    nodeItems[2],
                    nodeItems[3],
                    nodeItems[4],
                    linkValue);
                    Items.Add(downloadItem);
                
                }
                catch (Exception e)
                {
                }
            }
        }
        
        
        
    }
}