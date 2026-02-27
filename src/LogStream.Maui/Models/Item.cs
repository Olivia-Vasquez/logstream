using System;
using SQLite;

namespace LogStream.Models;

[Table("Items")]
public class Item
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // e.g. "app-2026-02-19.log"
    [Indexed]
    public string FileName { get; set; } = string.Empty;

    // When the file was imported into the app/db
    [Indexed]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int DetailCount { get; set; }
}
