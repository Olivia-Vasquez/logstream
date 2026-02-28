using System;
using System.IO;
using LogStream.Core.Models;
using LogStream.Core.Services;

namespace LogStream.Core.Parsing
{
    public class FileLogParser : ILogParser
    {
        private readonly ILogsDatabase _database;

        public FileLogParser(ILogsDatabase database)
        {
            _database = database;
        }

        public void Parse(string filePath)
        {
            var item = new Item
            {
                FileName = Path.GetFileName(filePath),
                CreatedAt = DateTime.UtcNow,
                DetailCount = 0
            };

            _database.InsertItemAsync(item).Wait();
            string[] logLines = File.ReadAllLines(filePath);
            int lineNumber = 1;
            foreach (var line in logLines)
            {
                var trimmed = line.Trim();
                // Example format: [2026-02-19 14:23:45] INFO: Sample log message
                var parts = trimmed.Split(' ', 3);
                if (parts.Length < 3) continue;

                DateTime? timestamp = null;
                string level = string.Empty;
                string message = string.Empty;

                try
                {
                    var tsRaw = parts[0].Trim('[', ']');
                    timestamp = DateTime.Parse(tsRaw);
                    level = parts[1].Replace(":", "");
                    message = parts[2];
                }
                catch
                {
                    // If parsing fails, we can skip this line or log it as a raw entry
                    message = trimmed; // Fallback to raw line as message
                    timestamp = null;
                    level = string.Empty;           
                }

                var detail = new ItemDetail
                {
                    ItemId = item.Id,
                    LineNumber = lineNumber++,
                    Timestamp = timestamp,
                    Level = level,
                    Message = message,
                    Raw = line
                };
                item.DetailCount++;
                _database.InsertItemDetailAsync(detail).Wait();
            }
            _database.UpdateItemAsync(item).Wait();
        }

        public bool CanParse(string filePath)
        {
            if (!File.Exists(filePath) || Path.GetExtension(filePath).ToLower() != ".log")
                return false;
            string[] logLines = File.ReadAllLines(filePath);
            if (logLines.Length == 0 || !logLines[0].StartsWith("[") || !logLines[0].Contains("]"))
                return false;
            return true;
        }
    }
}