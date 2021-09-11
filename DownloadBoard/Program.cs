
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DownloadBoard
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var cookie = "";
            using var worker = new HttpWorker(cookie);
            await worker.StartFind(1, 100000);
            var items = HttpWorker.Items;
            var orderedItems = items.OrderBy(x => x.DateStart);
            Console.WriteLine("Работа с http завершена, начинаю Excel");
            var excelWorker = new ExcelWorker();
            excelWorker.Worker(orderedItems.ToArray());
        }
    }
}