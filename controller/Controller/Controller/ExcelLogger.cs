using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
namespace Controller
{
    public class ExcelLogger : IExcelLogger
    {
        private object locker = new object();
        private static object syncRoot = new object();
        public const string SheetName = "LOG";
        public ExcelLogger()
        {}
        public void LogJsonFromPOOL(string msg)
        {
            LogJson(msg, "FROM");
        }
        public void LogJson(string msg, string dirfrompool)
        {
            lock (locker)
            {
                using (ExcelPackage excelPackage = new ExcelPackage(Program.JsonLog))
                {
                    ExcelWorksheet worksheet;
                    if (excelPackage.Workbook.Worksheets.Count <= 0)
                    {
                        worksheet = excelPackage.Workbook.Worksheets.Add(SheetName);
                    }
                    else
                    {
                        worksheet = excelPackage.Workbook.Worksheets[0];
                    }
                    int numrow = worksheet.Dimension.Rows;
                    worksheet.Cells[++numrow, 1].Value = dirfrompool;
                    worksheet.Cells[numrow, 2].Value = msg;
                    FileInfo fi = new FileInfo(Program.JsonLog);
                    excelPackage.SaveAs(fi);
                }
            }
        }
        public void LogJsonTOPOOL(String msg)
        {
            LogJson(msg, "TO");
        }
        private static ExcelLogger _lgr;
        public static ExcelLogger  ExcelLog 
        {
            get
            {
                if (_lgr is null)
                {
                    lock (syncRoot)
                    {
                        if (_lgr is null)
                        {
                            _lgr = new ExcelLogger();
                        }
                    }
                }
                return _lgr;
            }
        }
    }
}