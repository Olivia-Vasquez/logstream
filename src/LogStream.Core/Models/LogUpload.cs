using System;

namespace LogStream.Core.Models
{
    /// <summary>
    /// Represents a log file upload.
    /// </summary>
    public class LogUpload
    {
        /// <summary>
        /// Unique identifier for the upload.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the uploaded file.
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// UTC timestamp when the upload was created.
        /// </summary>
        public DateTimeOffset CreatedUtc { get; set; }

        /// <summary>
        /// Optional line count of the log file.
        /// </summary>
        public int? LineCount { get; set; }

        /// <summary>
        /// Optional notes about the upload.
        /// </summary>
        public string? Notes { get; set; }
    }
}
