namespace WebServerForensics.Models
{
    public class SiteBinding
    {
        public string Protocol { get; set; }
        public string HostName { get; set; }
        public string IpAddress { get; set; }
        public string Port { get; set; }
        public string CertificateName { get; set; }
    }
}