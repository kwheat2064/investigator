using System.Linq;
using WebServerForensics.Formatters;
using WebServerForensics.Models;

namespace WebServerForensics.Reporters
{
    public class SummaryReporter : IReporter
    {
        public void WriteReport(Server server, FormatterManager formatterManager)
        {
            formatterManager.InitializeSection("Summary");
            formatterManager.InitializeRecord();
            formatterManager.WriteValue("Server Name", server.Name);
            formatterManager.WriteValue("# of App Pools", server.AppPools.Count.ToString());
            formatterManager.WriteValue("# of Sites", server.Sites.Count.ToString());
            formatterManager.WriteValue("# of Bindings with HTTPS", server.Sites.Select(site => site.Bindings.Count(binding => binding.Protocol.ToLower() == "https")).Sum().ToString());
        }
    }
}