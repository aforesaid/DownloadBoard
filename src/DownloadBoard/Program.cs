using System;
using System.Linq;
using System.Threading.Tasks;
using DownloadBoard.Models;
using DownloadBoard.Services;
using Microsoft.Extensions.Configuration;

namespace DownloadBoard
{
    static class Program
    {
        static async Task Main()
        {
            var configuration = GetConfiguration();

            using var mireaApiClient = new MireaApiClient(configuration["Cookie"]);
            var items = await mireaApiClient.GetWebinarsFromApi(sort: true);

            const string fileName = "webinars";
            ExcelHelper.SaveAll(items, fileName);
        }
        
        private static IConfigurationRoot GetConfiguration()
            => new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .Build();
        }
}