
using System;

namespace DownloadBoard
{
    class Program
    {
        static void Main(string[] args)
        {
            var cookie = "";
            var worker = new HttpWorker(cookie);
            worker.StartFind(1, 50000);
            var items = HttpWorker.Items;
            Console.WriteLine("Работа с http завершена, начинаю Excel");
            var excelWorker = new ExcelWorker();
            excelWorker.Worker(items.ToArray());
        }
    }
}