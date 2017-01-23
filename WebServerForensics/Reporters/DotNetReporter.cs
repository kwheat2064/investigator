using WebServerForensics.Formatters;
using WebServerForensics.Models;

namespace WebServerForensics.Reporters
{
    public class DotNetReporter : IReporter
    {
        public void WriteReport(Server server, FormatterManager formatterManager)
        {
            formatterManager.InitializeSection(".NET");
            foreach (var site in server.Sites)
            {
                foreach (var directory in site.VirtualDirectories)
                {
                    formatterManager.InitializeRecord();
                    formatterManager.WriteValue("Site Name", site.Name);
                    formatterManager.WriteValue("Path", directory.Path);
                    formatterManager.WriteValue("Site Name", site.Name);
                    formatterManager.WriteValue("Path", directory.Path);
                    formatterManager.WriteValue("Physical Path", directory.PhysicalPath);
                    formatterManager.WriteValue("Authentication Mode", directory.AuthenticationMode);
                    formatterManager.WriteValue("Target .NET Framework", directory.TargetDotNetFramework);
                    formatterManager.WriteValue("Debug Enabled", directory.DebugEnabled.ToString());
                    formatterManager.WriteValue("Reveals Stock Error Pages", directory.RevealsStockErrorPages.ToString());
                    formatterManager.WriteValue("Reveals Error Urls", directory.RevealsErrorUrls.ToString());
                    formatterManager.WriteValue("Reveals ASP.NET Version Header", directory.RevealsAspNetVersionHeader.ToString());
                    formatterManager.WriteValue("Request Validation Disabled", directory.RequestValidationDisabled.ToString());
                    formatterManager.WriteValue("JavaScript Can Access Cookies", directory.JavaScriptCanAccessCookies.ToString());
                    formatterManager.WriteValue("Insecure Cookies Allowed", directory.InsecureCookiesAllowed.ToString());
                    formatterManager.WriteValue("Cookieless Sessions Allowed", directory.CookielessSessionsAllowed.ToString());
                    formatterManager.WriteValue("Trace Publicly Enabled", directory.TracePubliclyEnabled.ToString());
                }
            }
        }
    }
}