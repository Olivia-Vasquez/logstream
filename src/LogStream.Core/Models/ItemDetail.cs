using System;

namespace LogStream.Core.Models
{
    public class ItemDetail
    {
        public int Id { get; set; }

        // Foreign key to Item
        public int ItemId { get; set; }

        // Order within the file
        public int LineNumber { get; set; }

        // Parsed fields
        public DateTime? Timestamp { get; set; }

        public string Level { get; set; } = string.Empty; // "INFO", "WARN", "ERROR", etc.

        public string Message { get; set; } = string.Empty;

        public string Raw { get; set; } = string.Empty;
    }
}