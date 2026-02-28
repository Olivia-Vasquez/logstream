using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SQLite;
using LogStream.Core.Models;
using LogStream.Core.Services;

namespace LogStream.Core.Services;

public sealed class LogsDatabase : ILogsDatabase
{
    private readonly SQLiteAsyncConnection _db;

    private bool _initialized;
    private readonly SemaphoreSlim _initLock = new(1, 1);

    public LogsDatabase(string dbPath)
    {
        _db = new SQLiteAsyncConnection(
            dbPath,
            SQLiteOpenFlags.ReadWrite |
            SQLiteOpenFlags.Create |
            SQLiteOpenFlags.SharedCache
        );
    }

    private async Task EnsureInitializedAsync()
    {
        if (_initialized) return;

        await _initLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (_initialized) return;

            await _db.CreateTableAsync<Item>().ConfigureAwait(false);
            await _db.CreateTableAsync<ItemDetail>().ConfigureAwait(false);

            // Indexes (safe to run repeatedly)
            await _db.ExecuteAsync(
                "CREATE INDEX IF NOT EXISTS idx_items_createdat ON Items (CreatedAt)"
            ).ConfigureAwait(false);

            await _db.ExecuteAsync(
                "CREATE INDEX IF NOT EXISTS idx_items_filename ON Items (FileName)"
            ).ConfigureAwait(false);

            await _db.ExecuteAsync(
                "CREATE INDEX IF NOT EXISTS idx_itemdetails_itemid_linenumber ON ItemDetails (ItemId, LineNumber)"
            ).ConfigureAwait(false);

            await _db.ExecuteAsync(
                "CREATE INDEX IF NOT EXISTS idx_itemdetails_itemid_timestamp ON ItemDetails (ItemId, Timestamp)"
            ).ConfigureAwait(false);

            _initialized = true;
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async Task<int> InsertItemAsync(Item item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        await EnsureInitializedAsync().ConfigureAwait(false);
        return await _db.InsertAsync(item).ConfigureAwait(false);
    }

    public async Task<int> InsertItemDetailAsync(ItemDetail detail)
    {
        if (detail is null) throw new ArgumentNullException(nameof(detail));

        await EnsureInitializedAsync().ConfigureAwait(false);
        return await _db.InsertAsync(detail).ConfigureAwait(false);
    }

    // Bulk insert is important for log files
    public async Task<int> InsertItemDetailsAsync(IEnumerable<ItemDetail> details)
    {
        if (details is null) throw new ArgumentNullException(nameof(details));

        await EnsureInitializedAsync().ConfigureAwait(false);

        var list = details as IList<ItemDetail> ?? details.ToList();
        if (list.Count == 0) return 0;

        // RunInTransactionAsync is synchronous inside; it’s still the common pattern with sqlite-net
        await _db.RunInTransactionAsync(conn =>
        {
            foreach (var d in list)
                conn.Insert(d);
        }).ConfigureAwait(false);

        return list.Count;
    }

    public async Task<List<Item>> GetItemsAsync()
    {
        await EnsureInitializedAsync().ConfigureAwait(false);
        return await _db.Table<Item>()
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task<List<ItemDetail>> GetItemDetailsAsync(int itemId)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);
        return await _db.Table<ItemDetail>()
            .Where(d => d.ItemId == itemId)
            .OrderByDescending(d => d.Timestamp)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task<int> DeleteItemAsync(Item item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        await EnsureInitializedAsync().ConfigureAwait(false);

        // manual cascade
        await _db.Table<ItemDetail>()
            .DeleteAsync(d => d.ItemId == item.Id)
            .ConfigureAwait(false);

        return await _db.DeleteAsync(item).ConfigureAwait(false);
    }

    public async Task<int> UpdateItemAsync(Item item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        await EnsureInitializedAsync().ConfigureAwait(false);
        return await _db.UpdateAsync(item).ConfigureAwait(false);
    }
}
