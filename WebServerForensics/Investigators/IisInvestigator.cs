using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using Microsoft.Web.Administration;
using WebServerForensics.Models;
using WebConfigurationManager = System.Web.Configuration.WebConfigurationManager;

namespace WebServerForensics.Investigators
{
    public class IisInvestigator
    {
        //this documentation is useful for understanding the interactions with IIS
        //https://msdn.microsoft.com/en-us/library/microsoft.web.administration(v=vs.90).aspx

        private readonly Server _server = new Server();

        public Server Run()
        {
            _server.Name = Environment.MachineName;

            using (var localServer = new ServerManager())
            {
                InvestigateApplicationPools(localServer);
                InvestigateSites(localServer);
                InvestigateDotNet(localServer);
                InvestigateJavaScriptAndCss();
            }

            return _server;
        }

        private void InvestigateApplicationPools(ServerManager localServer)
        {
            //make list of application pools
            foreach (var appPool in localServer.ApplicationPools)
                _server.AppPools.Add(new AppPool
                {
                    Name = appPool.Name,
                    AutoStart = appPool.AutoStart,
                    PipelineMode = appPool.ManagedPipelineMode.ToString(),
                    RuntimeVersion = appPool.ManagedRuntimeVersion,
                    IdentityType = appPool.ProcessModel.IdentityType.ToString(),
                    UserName = appPool.ProcessModel.UserName,
                    CurrentState = appPool.State.ToString()
                });
        }

        private void InvestigateSites(ServerManager localServer)
        {
            foreach (var site in localServer.Sites)
            {
                //make a list of the applications
                var virtualDirs = site.Applications.ToList().SelectMany(app =>
                    app.VirtualDirectories.Select(dir => new VirtualDir
                    {
                        AppPath = app.Path,
                        AppPool = app.ApplicationPoolName,
                        Path = dir.Path,
                        PhysicalPath = !string.IsNullOrWhiteSpace(dir.PhysicalPath) ? Environment.ExpandEnvironmentVariables(dir.PhysicalPath) : null
                    }).ToList()
                ).ToList();

                //make a list of bindings
                var siteBindings = site.Bindings.Select(binding =>
                {
                    var certName = string.Empty;
                    if (binding.Protocol.ToLower() == "https")
                    {
                        var store = new X509Store(binding.CertificateStoreName);
                        store.Open(OpenFlags.ReadOnly);
                        //todo add certificate validity checking...
                        var cert = store.Certificates.Find(X509FindType.FindByThumbprint, binding.CertificateHash, true)
                            .OfType<X509Certificate2>()
                            .FirstOrDefault();
                        certName = cert != null ? cert.FriendlyName : "No valid certificate found!";
                    }

                    var bindingInfo = binding.BindingInformation.Split(new[] { ':' }, StringSplitOptions.None);

                    return new SiteBinding
                    {
                        Protocol = binding.Protocol,
                        HostName = binding.Host,
                        IpAddress = bindingInfo[0],
                        Port = bindingInfo[1],
                        CertificateName = certName
                    };
                }).ToList();

                //assemble site including applications and bindings
                _server.Sites.Add(new Website
                {
                    Name = site.Name,
                    VirtualDirectories = virtualDirs,
                    Bindings = siteBindings,
                    CurrentState = site.State.ToString(),
                    AutoStart = site.ServerAutoStart
                });
            }
        }

        private void InvestigateDotNet(ServerManager localServer)
        {
            //todo find any commercial deployments???
            //todo detect windows services???
            //todo get app type: Webforms, MVC, WebAPI
            //todo detect sizes of files and directories: app DLLs, all DLLs, all HTML/JS/CSS, whole app, logs)

            foreach (var site in _server.Sites)
            {
                foreach (var dir in site.VirtualDirectories)
                {
                    //load up web.config
                    var virtualDirectoryMapping = new VirtualDirectoryMapping(Environment.ExpandEnvironmentVariables(dir.PhysicalPath), true, "web.config");
                    var fileMap = new WebConfigurationFileMap();
                    fileMap.VirtualDirectories.Add(dir.Path, virtualDirectoryMapping);
                    var webConfig = WebConfigurationManager.OpenMappedWebConfiguration(fileMap, dir.Path, site.Name);

                    //how to work with this webConfig: https://msdn.microsoft.com/en-us/library/system.web.configuration(v=vs.110).aspx

                    var connectionStrings = webConfig.ConnectionStrings.ConnectionStrings;
                    dir.Databases = connectionStrings.Cast<ConnectionStringSettings>().Select(connectionString => new Database
                    {
                        ConnectionName = connectionString.Name,
                        ConnectionString = connectionString.ConnectionString,
                        Provider = connectionString.ProviderName
                    }).ToList();

                    var authSection = (AuthenticationSection) webConfig.GetSection("system.web/authentication");
                    dir.AuthenticationMode = authSection.Mode.ToString();
                    //if more auth info is needed for forms auth, start grabbing things off of the authSection.Forms...
                    //dir.Auth = authSection.Forms.

                    //digging up security issues. refer to OWASP guidelines
                    //http://www.developerfusion.com/article/6678/top-10-application-security-vulnerabilities-in-webconfig-files-part-one/
                    //https://www.troyhunt.com/owasp-top-10-for-net-developers-part-2/ <-- look at the whole series

                    var compilationSection = (CompilationSection) webConfig.GetSection("system.web/compilation");
                    dir.TargetDotNetFramework = compilationSection.TargetFramework;
                    dir.DebugEnabled = compilationSection.Debug;

                    var customErrorsSection = (CustomErrorsSection) webConfig.GetSection("system.web/customErrors");
                    dir.RevealsStockErrorPages = customErrorsSection.Mode == CustomErrorsMode.Off;
                    dir.RevealsErrorUrls = customErrorsSection.RedirectMode == CustomErrorsRedirectMode.ResponseRedirect;

                    var traceSection = (TraceSection) webConfig.GetSection("system.web/trace");
                    dir.TracePubliclyEnabled = traceSection.Enabled && !traceSection.LocalOnly;

                    var httpRuntimeSection = (HttpRuntimeSection) webConfig.GetSection("system.web/httpRuntime");
                    dir.RevealsAspNetVersionHeader = httpRuntimeSection.EnableVersionHeader;

                    var pagesSection = (PagesSection) webConfig.GetSection("system.web/pages");
                    dir.RequestValidationDisabled = !pagesSection.ValidateRequest;

                    var cookiesSection = (HttpCookiesSection) webConfig.GetSection("system.web/httpCookies");
                    dir.JavaScriptCanAccessCookies = !cookiesSection.HttpOnlyCookies;
                    dir.InsecureCookiesAllowed = !cookiesSection.RequireSSL;

                    var sessionStateSection = (SessionStateSection) webConfig.GetSection("system.web/sessionState");
                    dir.CookielessSessionsAllowed = sessionStateSection.Cookieless != HttpCookieMode.UseCookies;
                }
            }
        }

        private void InvestigateJavaScriptAndCss()
        {
            //todo add total js size?

            var angular1Regex = new Regex(@"angularjs\.org");
            var reactRegex = new Regex(@"\* React v");
            var emberRegex = new Regex(@"Ember\.imports|this\.Ember");
            var knockoutRegex = new Regex(@"(?:\/\/|\*) Knockout JavaScript library");
            var backboneRegex = new Regex(@"\.Backbone");
            var jQueryRegex = new Regex(@"jquery.org/license");
            var underscoreOrLodashRegex = new Regex(@"\._[ ]?=");
            var bootstrapRegex = new Regex(@"\* Bootstrap v");

            foreach (var site in _server.Sites)
            {
                foreach (var virtualDir in site.VirtualDirectories)
                {
                    if (virtualDir.PhysicalPath != null && Directory.Exists(virtualDir.PhysicalPath))
                    {
                        //detect libraries in JS and CSS files
                        var jsFiles = Directory.GetFileSystemEntries(virtualDir.PhysicalPath, "*.js", SearchOption.AllDirectories).Where(x => !string.IsNullOrWhiteSpace(Path.GetFileName(x))).ToList();
                        var cssFiles = Directory.GetFileSystemEntries(virtualDir.PhysicalPath, "*.css", SearchOption.AllDirectories).Where(x => !string.IsNullOrWhiteSpace(Path.GetFileName(x))).ToList();

                        foreach (var jsFile in jsFiles)
                        {
                            var lines = File.ReadLines(jsFile);
                            foreach (var line in lines)
                            {
                                if (angular1Regex.IsMatch(line)) virtualDir.FoundAngular1 = true;
                                if (reactRegex.IsMatch(line)) virtualDir.FoundReact = true;
                                if (emberRegex.IsMatch(line)) virtualDir.FoundEmber = true;
                                if (knockoutRegex.IsMatch(line)) virtualDir.FoundKnockout = true;
                                if (backboneRegex.IsMatch(line)) virtualDir.FoundBackbone = true;
                                if (jQueryRegex.IsMatch(line)) virtualDir.FoundjQuery = true;
                                if (underscoreOrLodashRegex.IsMatch(line)) virtualDir.FoundUnderscoreOrLodash = true;
                            }
                        }

                        foreach (var cssFile in cssFiles)
                        {
                            var lines = File.ReadLines(cssFile);
                            foreach (var line in lines)
                            {
                                if (bootstrapRegex.IsMatch(line)) virtualDir.FoundBootstrap = true;
                            }
                        }
                    }
                }
            }
        }
    }
}