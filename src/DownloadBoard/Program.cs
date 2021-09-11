
using System;
using System.Linq;
using System.Threading.Tasks;
using DownloadBoard.Services;

namespace DownloadBoard
{
    class Program
    {
        static async Task Main()
        {
            var cookie = "";
            using var worker = new MireaApiClient(cookie);
            await worker.StartFind(1, 100000);
            var items = MireaApiClient.Items;
            var orderedItems = items.OrderBy(x => x.DateStart);
            Console.WriteLine("Работа с http завершена, начинаю Excel");
            var excelWorker = new ExcelClient();
            excelWorker.Worker(orderedItems.ToArray());
        }
    }
}