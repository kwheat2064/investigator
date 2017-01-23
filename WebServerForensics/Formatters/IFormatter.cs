namespace WebServerForensics.Formatters
{
    public interface IFormatter
    {
        void InitializeSection(string sectionName);
        void InitializeRecord();
        void WriteValue(string key, string value);
        void End();
    }
}