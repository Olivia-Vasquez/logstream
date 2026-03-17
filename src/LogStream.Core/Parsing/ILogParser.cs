namespace LogStream.Core.Parsing
{
    public interface ILogParser
    {
        Task ParseAsync(string filePath);
        bool CanParse(string filePath);
    }
}