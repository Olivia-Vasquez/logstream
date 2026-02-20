using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SQLite;
using LogStream.Models;

namespace LogStream.Services;

public sealed class LogsDatabase
{
    private readonly SQLiteAsyncConnection _db;

    // one-time init gate (prevents deadlocks + ensures table exists before use)
    private bool _initialized;
    private readonly SemaphoreSlim _initLock = new(1, 1);

    public LogsDatabase(string dbPath)
    {
        // Some default flags that are good for mobile platforms
        _db = new SQLiteAsyncConnection(
            dbPath,
            SQLiteOpenFlags.ReadWrite |
            SQLiteOpenFlags.Create |
            SQLiteOpenFlags.SharedCache
        );
    }

    // Ensure the database is initialized (tables created, etc.) before any operations
    private async Task EnsureInitializedAsync()
    {
        if (_initialized) return;

        await _initLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (_initialized) return;

            // CreateTableAsync already uses "IF NOT EXISTS"
            await _db.CreateTableAsync<LogEntry>().ConfigureAwait(false);

            // Optional: helpful indexes (safe to call repeatedly if you want)
            await _db.ExecuteAsync(
                "CREATE INDEX IF NOT EXISTS idx_logentry_createdat ON LogEntry (CreatedAt)"
            ).ConfigureAwait(false);

            await _db.ExecuteAsync(
                "CREATE INDEX IF NOT EXISTS idx_logentry_filename ON LogEntry (FileName)"
            ).ConfigureAwait(false);

            _initialized = true;
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async Task<int> SaveLogAsync(LogEntry entry)
    {
        if (entry is null) throw new ArgumentNullException(nameof(entry));

        await EnsureInitializedAsync().ConfigureAwait(false);
        return await _db.InsertAsync(entry).ConfigureAwait(false);
    }

    public async Task<List<LogEntry>> GetLogsAsync()
    {
        await EnsureInitializedAsync().ConfigureAwait(false);
        return await _db.Table<LogEntry>()
                        .OrderByDescending(e => e.CreatedAt)
                        .ToListAsync()
                        .ConfigureAwait(false);
    }

    public async Task<int> DeleteLogAsync(LogEntry entry)
    {
        if (entry is null) throw new ArgumentNullException(nameof(entry));

        await EnsureInitializedAsync().ConfigureAwait(false);
        return await _db.DeleteAsync(entry).ConfigureAwait(false);
    }

    public async Task<int> DeleteAllAsync()
    {
        await EnsureInitializedAsync().ConfigureAwait(false);
        return await _db.DeleteAllAsync<LogEntry>().ConfigureAwait(false);
    }
}
