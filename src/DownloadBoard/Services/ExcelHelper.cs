using System;
using System.IO;
using DownloadBoard.Models;
using OfficeOpenXml;

namespace DownloadBoard.Services
{
    public class ExcelHelper
    {
        public static void SaveAll(Webinar[] items, string fileName)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var excelPackage = new ExcelPackage();
            var worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
            
            worksheet.Cells[1,1].Value = "Id";
            worksheet.Cells[1,2].Value = "WebName";
            worksheet.Cells[1,3].Value = "Status";
            worksheet.Cells[1,4].Value = "DateStart";
            worksheet.Cells[1,5].Value = "DateEnd";
            worksheet.Cells[1,6].Value = "Link";

            for (var i = 0; i < items.Length; i++)
            {
                var worksheetIndex = i + 2;
                worksheet.Cells[worksheetIndex, 1].Value = items[i].Id;
                worksheet.Cells[worksheetIndex,2].Value = items[i].WebName;
                worksheet.Cells[worksheetIndex,3].Value = items[i].Status;
                worksheet.Cells[worksheetIndex,4].Value = items[i].DateStart;
                worksheet.Cells[worksheetIndex,5].Value = items[i].DateEnd;
                worksheet.Cells[worksheetIndex,6].Value = items[i].Link;
            }

            var path = AppDomain.CurrentDomain.BaseDirectory + $"ParseData/{fileName}.xlsx";
            var fi = new FileInfo(path);
            
            excelPackage.SaveAs(fi);
        }
    }
}