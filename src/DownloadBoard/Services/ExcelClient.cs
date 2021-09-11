using System;
using System.IO;
using DownloadBoard.Models;
using OfficeOpenXml;

namespace DownloadBoard.Services
{
    public class ExcelClient
    {
        public void Worker(Webinar[] items)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var excelPackage = new ExcelPackage();
            var worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");

            //Add some text to cell A1
            worksheet.Cells[1,1].Value = "Id";
            worksheet.Cells[1,2].Value = "WebName";
            worksheet.Cells[1,3].Value = "Status";
            worksheet.Cells[1,4].Value = "DateStart";
            worksheet.Cells[1,5].Value = "DateEnd";
            worksheet.Cells[1,6].Value = "Link";

            for (int i = 0; i < items.Length; i++)
            {
                worksheet.Cells[i+2, 1].Value = items[i].Id;
                worksheet.Cells[i+2,2].Value = items[i].WebName;
                worksheet.Cells[i+2,3].Value = items[i].Status;
                worksheet.Cells[i+2,4].Value = items[i].DateStart;
                worksheet.Cells[i+2,5].Value = items[i].DateEnd;
                worksheet.Cells[i+2,6].Value = items[i].Link;
            }
            
            var fi = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "result.xlsx");
            excelPackage.SaveAs(fi);
        }
    }
}