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

        public async Task ParseAsync(string filePath)
        {
            var item = new Item
            {
                FileName = Path.GetFileName(filePath),
                CreatedAt = DateTime.UtcNow,
                DetailCount = 0
            };

            await _database.InsertItemAsync(item).ConfigureAwait(false);
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
                catch (FormatException)
                {
                    // If parsing fails, fall back to raw line as message
                    message = trimmed;
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
                await _database.InsertItemDetailAsync(detail).ConfigureAwait(false);
            }
            await _database.UpdateItemAsync(item).ConfigureAwait(false);
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