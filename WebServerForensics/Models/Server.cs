using System.Collections.Generic;

namespace WebServerForensics.Models
{
    public class Server
    {
        public List<AppPool> AppPools { get; set; }
        public List<Website> Sites { get; set; }
        public string Name { get; set; }

        public Server()
        {
            AppPools = new List<AppPool>();
            Sites = new List<Website>();
        }
    }
}