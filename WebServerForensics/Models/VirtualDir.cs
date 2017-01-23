using System.Collections.Generic;

namespace WebServerForensics.Models
{
    public class VirtualDir
    {
        public string Path { get; set; }
        public string PhysicalPath { get; set; }
        public string AppPath { get; set; }
        public string AppPool { get; set; }
        public bool FoundAngular1 { get; set; }
        public bool FoundReact { get; set; }
        public bool FoundEmber { get; set; }
        public bool FoundKnockout { get; set; }
        public bool FoundBackbone { get; set; }
        public bool FoundjQuery { get; set; }
        public bool FoundUnderscoreOrLodash { get; set; }
        public bool FoundBootstrap { get; set; }
        public List<Database> Databases { get; set; }
        public string AuthenticationMode { get; set; }
        public string TargetDotNetFramework { get; set; }
        public bool DebugEnabled { get; set; }
        public bool RevealsStockErrorPages { get; set; }
        public bool RevealsAspNetVersionHeader { get; set; }
        public bool RequestValidationDisabled { get; set; }
        public bool JavaScriptCanAccessCookies { get; set; }
        public bool InsecureCookiesAllowed { get; set; }
        public bool CookielessSessionsAllowed { get; set; }
        public bool RevealsErrorUrls { get; set; }
        public bool TracePubliclyEnabled { get; set; }

        public VirtualDir()
        {
            Databases = new List<Database>();
        }
    }
}