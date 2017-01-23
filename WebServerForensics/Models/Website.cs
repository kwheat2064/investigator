using System.Collections.Generic;
using Microsoft.Web.Administration;

namespace WebServerForensics.Models
{
    public class Website
    {
        public string Name { get; set; }
        public List<VirtualDir> VirtualDirectories { get; set; }
        public List<SiteBinding> Bindings { get; set; }
        public string CurrentState { get; set; }
        public bool AutoStart { get; set; }
    }
}