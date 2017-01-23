using WebServerForensics.Formatters;
using WebServerForensics.Models;

namespace WebServerForensics.Reporters
{
    public class SiteReporter : IReporter
    {
        public void WriteReport(Server server, FormatterManager formatterManager)
        {
            formatterManager.InitializeSection("Sites");
            foreach (var site in server.Sites)
            {
                formatterManager.InitializeRecord();
                formatterManager.WriteValue("Name", site.Name);
                formatterManager.WriteValue("Current State", site.CurrentState);
                formatterManager.WriteValue("Auto Start", site.AutoStart.ToString());
                formatterManager.WriteValue("# of Bindings", site.Bindings.Count.ToString());
                formatterManager.WriteValue("# of Virtual Directories", site.VirtualDirectories.Count.ToString());
            }

            formatterManager.InitializeSection("Bindings");
            foreach (var site in server.Sites)
            {
                foreach (var binding in site.Bindings)
                {
                    formatterManager.InitializeRecord();
                    formatterManager.WriteValue("Site Name", site.Name);
                    formatterManager.WriteValue("Protocol", binding.Protocol);
                    formatterManager.WriteValue("IP Address", binding.IpAddress);
                    formatterManager.WriteValue("Port", binding.Port);
                    formatterManager.WriteValue("Host Name", binding.HostName);
                    formatterManager.WriteValue("Certificate Name", binding.CertificateName);
                }
            }

            formatterManager.InitializeSection("Virtual Directories");
            foreach (var site in server.Sites)
            {
                foreach (var directory in site.VirtualDirectories)
                {
                    formatterManager.InitializeRecord();
                    formatterManager.WriteValue("Site Name", site.Name);
                    formatterManager.WriteValue("Path", directory.Path);
                    formatterManager.WriteValue("Physical Path", directory.PhysicalPath);
                    formatterManager.WriteValue("Application Pool", directory.AppPool);
                    formatterManager.WriteValue("Application Path", directory.AppPath);
                }
            }
        }
    }
}