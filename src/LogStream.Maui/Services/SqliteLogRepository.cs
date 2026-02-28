using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LogStream.Core.Abstractions;
using LogStream.Core.Models;
using LogStream.Core.Services;

namespace LogStream.Maui.Services
{
    /// <summary>
    /// SQLite-backed implementation of ILogRepository for MAUI.
    /// </summary>
    public class SqliteLogRepository : ILogRepository
    {
        private readonly LogsDatabase _db;

        public SqliteLogRepository(LogsDatabase db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<LogUpload>> GetUploadsAsync(CancellationToken ct = default)
        {
            // TODO: Map Items to LogUpload
            var items = await _db.GetItemsAsync();
            return items.Select(i => new LogUpload
            {
                Id = Guid.TryParse(i.Guid, out var guid) ? guid : Guid.NewGuid(),
                FileName = i.FileName,
                CreatedUtc = i.CreatedAt,
                LineCount = i.LineCount,
                Notes = i.Notes
            }).ToList();
        }

        public async Task<LogUpload> CreateUploadAsync(LogUpload upload, CancellationToken ct = default)
        {
            // TODO: Map LogUpload to Item
            var item = new Item
            {
                Guid = upload.Id.ToString(),
                FileName = upload.FileName,
                CreatedAt = upload.CreatedUtc.UtcDateTime,
                LineCount = upload.LineCount,
                Notes = upload.Notes
            };
            await _db.InsertItemAsync(item);
            return upload;
        }

        public async Task DeleteUploadAsync(Guid uploadId, CancellationToken ct = default)
        {
            var items = await _db.GetItemsAsync();
            var item = items.FirstOrDefault(i => i.Guid == uploadId.ToString());
            if (item != null)
            {
                await _db.DeleteItemAsync(item);
            }
        }

        public async Task AddEntriesAsync(Guid uploadId, IEnumerable<LogEntry> entries, CancellationToken ct = default)
        {
            var items = await _db.GetItemsAsync();
            var item = items.FirstOrDefault(i => i.Guid == uploadId.ToString());
            if (item == null) return;
            var details = entries.Select(e => new ItemDetail
            {
                ItemId = item.Id,
                Message = e.Message,
                FileName = e.FileName,
                Timestamp = e.CreatedAt
            });
            await _db.InsertItemDetailsAsync(details);
        }

        public async Task<IReadOnlyList<LogEntry>> GetEntriesAsync(Guid uploadId, int skip = 0, int take = 200, CancellationToken ct = default)
        {
            var items = await _db.GetItemsAsync();
            var item = items.FirstOrDefault(i => i.Guid == uploadId.ToString());
            if (item == null) return new List<LogEntry>();
            var details = await _db.GetItemDetailsAsync(item.Id);
            return details.Skip(skip).Take(take).Select(d => new LogEntry
            {
                Id = d.Id,
                FileName = d.FileName,
                Message = d.Message,
                CreatedAt = d.Timestamp ?? DateTime.UtcNow
            }).ToList();
        }

        public async Task<IReadOnlyList<LogEntry>> SearchEntriesAsync(Guid uploadId, string query, int skip = 0, int take = 200, CancellationToken ct = default)
        {
            var items = await _db.GetItemsAsync();
            var item = items.FirstOrDefault(i => i.Guid == uploadId.ToString());
            if (item == null) return new List<LogEntry>();
            var details = await _db.GetItemDetailsAsync(item.Id);
            return details.Where(d => d.Message.Contains(query, StringComparison.OrdinalIgnoreCase))
                .Skip(skip).Take(take)
                .Select(d => new LogEntry
                {
                    Id = d.Id,
                    FileName = d.FileName,
                    Message = d.Message,
                    CreatedAt = d.Timestamp ?? DateTime.UtcNow
                }).ToList();
        }
    }
}
