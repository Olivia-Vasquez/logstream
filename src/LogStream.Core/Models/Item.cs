using System;

namespace LogStream.Core.Models
{
    public class Item
    {
        public int Id { get; set; }

        // Unique Guid for repository mapping
        public string Guid { get; set; }

        // e.g. "app-2026-02-19.log"
        public string FileName { get; set; } = string.Empty;

        // When the file was imported into the app/db
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int DetailCount { get; set; }

        // Optional line count
        public int? LineCount { get; set; }

        // Optional notes
        public string? Notes { get; set; }

        public Item()
        {
            Guid = System.Guid.NewGuid().ToString();
        }
    }
}
