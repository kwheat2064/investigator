using WebServerForensics.Formatters;
using WebServerForensics.Models;

namespace WebServerForensics.Reporters
{
    public class DatabaseReporter : IReporter
    {
        public void WriteReport(Server server, FormatterManager formatterManager)
        {
            formatterManager.InitializeSection("Databases");
            foreach (var site in server.Sites)
            {
                foreach (var directory in site.VirtualDirectories)
                {
                    foreach (var database in directory.Databases)
                    {
                        formatterManager.InitializeRecord();
                        formatterManager.WriteValue("Site Name", site.Name);
                        formatterManager.WriteValue("Path", directory.Path);
                        formatterManager.WriteValue("Connection Name", database.ConnectionName);
                        formatterManager.WriteValue("Provider", database.Provider);
                        formatterManager.WriteValue("Connection String", database.ConnectionString);
                    }
                }
            }
        }
    }
}