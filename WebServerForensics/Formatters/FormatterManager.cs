using System.Collections.Generic;

namespace WebServerForensics.Formatters
{
    public class FormatterManager : IFormatter
    {
        readonly IFormatter[] _formatters;

        public FormatterManager(IFormatter[] formatters)
        {
            _formatters = formatters;
        }

        public void InitializeSection(string sectionName)
        {
            foreach (var x in _formatters) x.InitializeSection(sectionName);
        }

        public void InitializeRecord()
        {
            foreach (var x in _formatters) x.InitializeRecord();
        }

        public void WriteValue(string key, string value)
        {
            foreach (var x in _formatters) x.WriteValue(key, value);
        }

        public void End()
        {
            foreach (var x in _formatters) x.End();
        }
    }
}