using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Web.Administration;
using WebServerForensics.Formatters;
using WebServerForensics.Investigators;
using WebServerForensics.Reporters;


namespace WebServerForensics
{
    class Program
    {
        static void Main(string[] args)
        {
            var iisInvestigator = new IisInvestigator();

            var server = iisInvestigator.Run();

            var formatters = new List<IFormatter>();
            
            //todo: use args to signal which formatters to use, custom file names, which reporters to use, etc
            formatters.Add(new ConsoleFormatter());

            var machineName = string.Join("_", Environment.MachineName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            formatters.Add(new SpreadsheetFormatter(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), $"{machineName}_forensic-report_{timestamp}.xlsx"));

            var formatterManager = new FormatterManager(formatters.ToArray());

            var reporters = new IReporter[]
            {
                new SummaryReporter(),
                new AppPoolReporter(),
                new SiteReporter(),
                new DotNetReporter(), 
                new DatabaseReporter(), 
                new JavaScriptReporter()
            };

            foreach (var reporter in reporters)
            {
                reporter.WriteReport(server, formatterManager);
            }

            formatterManager.End();

            Console.ReadLine();
        }
    }
}
