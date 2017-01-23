using System.IO;
using OfficeOpenXml;

namespace WebServerForensics.Formatters
{
    public class SpreadsheetFormatter : IFormatter
    {
        //this formatter uses the EPPlus library for creating Excel spreadsheets easily
        //http://epplus.codeplex.com/
        //this article is useful in understanding how to use the library
        //http://zeeshanumardotnet.blogspot.com/2011/06/creating-reports-in-excel-2007-using.html

        private readonly string _path;
        private readonly string _filename;
        private readonly ExcelPackage _excelPackage;
        private ExcelWorksheet _currentWorksheet;
        private int _currentRow = 1;
        private int _currentColumn = 1;

        public SpreadsheetFormatter(string path, string filename)
        {
            _path = path;
            _filename = filename;
            _excelPackage = new ExcelPackage();
        }

        public void InitializeSection(string sectionName)
        {
            _currentWorksheet = _excelPackage.Workbook.Worksheets.Add(sectionName);
            _currentRow = 1;
        }

        public void InitializeRecord()
        {
            _currentRow++;
            _currentColumn = 1;
        }

        public void WriteValue(string key, string value)
        {
            //if there is a header mismatch, move to the next column
            if (_currentWorksheet.Cells[1, _currentColumn].Value != null && _currentWorksheet.Cells[1, _currentColumn].Value.ToString() != key)
            {
                _currentColumn++;
                WriteValue(key, value);
            }
            else
            {
                //write the header
                if (_currentWorksheet.Cells[1, _currentColumn].Value == null)
                    _currentWorksheet.Cells[1, _currentColumn].Value = key;

                //write the cell value
                _currentWorksheet.Cells[_currentRow, _currentColumn].Value = value;

                //this hack makes sure that every column we touch is correctly autosized
                if (_currentRow == 2)
                    _currentWorksheet.Column(_currentColumn).AutoFit();
                
                _currentColumn++;
            }
        }

        public void End()
        {
            _excelPackage.SaveAs(new FileInfo(Path.Combine(_path, _filename)));
        }
    }
}