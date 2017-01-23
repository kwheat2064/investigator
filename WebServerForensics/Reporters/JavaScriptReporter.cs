using WebServerForensics.Formatters;
using WebServerForensics.Models;

namespace WebServerForensics.Reporters
{
    public class JavaScriptReporter : IReporter
    {
        public void WriteReport(Server server, FormatterManager formatterManager)
        {
            formatterManager.InitializeSection("JavaScript");
            foreach (var site in server.Sites)
            {
                foreach (var directory in site.VirtualDirectories)
                {
                    formatterManager.InitializeRecord();
                    formatterManager.WriteValue("Site Name", site.Name);
                    formatterManager.WriteValue("Path", directory.Path);
                    formatterManager.WriteValue("May Use Angular.js 1.x", directory.FoundAngular1.ToString());
                    formatterManager.WriteValue("May Use React.js", directory.FoundReact.ToString());
                    formatterManager.WriteValue("May Use Backbone.js", directory.FoundBackbone.ToString());
                    formatterManager.WriteValue("May Use Ember.js", directory.FoundEmber.ToString());
                    formatterManager.WriteValue("May Use Knockout.js", directory.FoundKnockout.ToString());
                    formatterManager.WriteValue("May Use jQuery", directory.FoundjQuery.ToString());
                    formatterManager.WriteValue("May Use Underscore/Lodash", directory.FoundUnderscoreOrLodash.ToString());
                    formatterManager.WriteValue("May Use Bootstrap CSS", directory.FoundBootstrap.ToString());
                }
            }
        }
    }
}