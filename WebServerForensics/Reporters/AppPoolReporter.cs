using WebServerForensics.Formatters;
using WebServerForensics.Models;

namespace WebServerForensics.Reporters
{
    public class AppPoolReporter : IReporter
    {
        public void WriteReport(Server server, FormatterManager formatterManager)
        {
            formatterManager.InitializeSection("Application Pools");
            foreach (var appPool in server.AppPools)
            {
                formatterManager.InitializeRecord();
                formatterManager.WriteValue("Name", appPool.Name);
                formatterManager.WriteValue("Current State", appPool.CurrentState);
                formatterManager.WriteValue("Runtime Version", appPool.RuntimeVersion);
                formatterManager.WriteValue("Pipeline Mode", appPool.PipelineMode);
                formatterManager.WriteValue("Auto Start", appPool.AutoStart.ToString());
                formatterManager.WriteValue("Username", appPool.UserName);
                formatterManager.WriteValue("Identity Type", appPool.IdentityType);
            }
        }
    }
}