using WebServerForensics.Formatters;
using WebServerForensics.Models;

namespace WebServerForensics.Reporters
{
    public interface IReporter
    {
        void WriteReport(Server server, FormatterManager formatterManager);
    }
}