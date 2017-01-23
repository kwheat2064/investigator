using System;

namespace WebServerForensics.Formatters
{
    public class ConsoleFormatter : IFormatter
    {
        private bool _firstSectionWritten;
  
        public void InitializeSection(string sectionName)
        {
            if (_firstSectionWritten)
            {
                Console.WriteLine();
                Console.WriteLine();
            }
            else
            {
                _firstSectionWritten = true;
            }

            Console.WriteLine("====================");
            Console.WriteLine("Section: {0}", sectionName);
        }

        public void InitializeRecord()
        {
            Console.WriteLine();
        }

        public void WriteValue(string key, string value)
        {
            Console.WriteLine("{0}: {1}", key, value);
        }

        public void End()
        {
        }
    }
}