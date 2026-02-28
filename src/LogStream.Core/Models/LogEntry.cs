using System;

namespace LogStream.Core.Models
{
    public class LogEntry
    {
        public int Id { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}