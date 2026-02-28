namespace LogStream.Core.Parsing
{
    public interface ILogParser
    {
        void Parse(string filePath);
        bool CanParse(string filePath);
    }
}