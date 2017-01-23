namespace WebServerForensics.Models
{
    public class AppPool
    {
        public string Name { get; set; }
        public bool AutoStart { get; set; }
        public string PipelineMode { get; set; }
        public string RuntimeVersion { get; set; }
        public string IdentityType { get; set; }
        public string UserName { get; set; }
        public string CurrentState { get; set; }
    }
}