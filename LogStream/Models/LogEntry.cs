using System;
using SQLite;

namespace LogStream.Models;

public class LogEntry
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


}