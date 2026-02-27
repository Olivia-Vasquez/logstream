using System;
using SQLite;

namespace LogStream.Models;

[Table("ItemDetails")]
public class ItemDetail
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Foreign key to Item
    [Indexed]
    public int ItemId { get; set; }

    // Order within the file
    [Indexed]
    public int LineNumber { get; set; }

    // Parsed fields
    [Indexed]
    public DateTime? Timestamp { get; set; }

    [Indexed]
    public string Level { get; set; } = string.Empty; // "INFO", "WARN", "ERROR", etc.

    public string Message { get; set; } = string.Empty;

    public string Raw { get; set; } = string.Empty;
}