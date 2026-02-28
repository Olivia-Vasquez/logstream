using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LogStream.Core.Models;

namespace LogStream.Core.Abstractions
{
    /// <summary>
    /// Abstraction for log data persistence and retrieval.
    /// </summary>
    public interface ILogRepository
    {
        /// <summary>
        /// Gets all log uploads.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>List of uploads.</returns>
        Task<IReadOnlyList<LogUpload>> GetUploadsAsync(CancellationToken ct = default);

        /// <summary>
        /// Creates a new log upload.
        /// </summary>
        /// <param name="upload">Upload to create.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Created upload.</returns>
        Task<LogUpload> CreateUploadAsync(LogUpload upload, CancellationToken ct = default);

        /// <summary>
        /// Deletes a log upload by ID.
        /// </summary>
        /// <param name="uploadId">Upload ID.</param>
        /// <param name="ct">Cancellation token.</param>
        Task DeleteUploadAsync(Guid uploadId, CancellationToken ct = default);

        /// <summary>
        /// Adds log entries to an upload.
        /// </summary>
        /// <param name="uploadId">Upload ID.</param>
        /// <param name="entries">Entries to add.</param>
        /// <param name="ct">Cancellation token.</param>
        Task AddEntriesAsync(Guid uploadId, IEnumerable<LogEntry> entries, CancellationToken ct = default);

        /// <summary>
        /// Gets log entries for an upload.
        /// </summary>
        /// <param name="uploadId">Upload ID.</param>
        /// <param name="skip">Entries to skip.</param>
        /// <param name="take">Entries to take.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>List of log entries.</returns>
        Task<IReadOnlyList<LogEntry>> GetEntriesAsync(Guid uploadId, int skip = 0, int take = 200, CancellationToken ct = default);

        /// <summary>
        /// Searches log entries for an upload.
        /// </summary>
        /// <param name="uploadId">Upload ID.</param>
        /// <param name="query">Search query.</param>
        /// <param name="skip">Entries to skip.</param>
        /// <param name="take">Entries to take.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>List of matching log entries.</returns>
        Task<IReadOnlyList<LogEntry>> SearchEntriesAsync(Guid uploadId, string query, int skip = 0, int take = 200, CancellationToken ct = default);
    }
}
