using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using __XamlGeneratedCode__;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LogStream.Core.Models;
using LogStream.Core.Services;
using LogStream.Core.Parsing;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Storage;
using ObjCRuntime;

namespace LogStream.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    private readonly LogStream.Core.Abstractions.ILogRepository _repository;
    private ILogParser? _logParser;

    [ObservableProperty]
    private ObservableCollection<LogUpload>? _uploads;
    [ObservableProperty]
    private ObservableCollection<LogEntry>? _entries;

    [ObservableProperty]
    private LogUpload? _selectedUpload;

    [ObservableProperty]
    private ObservableCollection<LogEntry>? _selectedUploadEntries;

    [ObservableProperty]
    private string? _fileName;

    [ObservableProperty]
    private string? _filePath;

    [ObservableProperty]
    private string? _filterText;

    public MainPageViewModel(LogStream.Core.Abstractions.ILogRepository repository)
    {
        _repository = repository;
        LoadUploads();
        // _logParser = new FileLogParser(_repository); // If parser needs repository
    }


    private void LoadUploads()
    {
        // Loads uploads from repository
        Console.WriteLine("Loading uploads from repository...");
        var uploads = _repository.GetUploadsAsync().Result;
        Uploads = new ObservableCollection<LogUpload>(uploads);
        Console.WriteLine($"Loaded {Uploads.Count} uploads from repository.");
    }

    [RelayCommand]
    private void CreateSampleLog()
    {
        Console.WriteLine("CreateSampleLog command executed.");
        var sampleFilePath = Path.Combine(FileSystem.AppDataDirectory, "sample_log.txt");
        File.WriteAllText(sampleFilePath, "This is a sample log entry.");
        Console.WriteLine($"Sample log file created at: {sampleFilePath}");
    }

    [RelayCommand]
    /// <summary>
    /// Uploads the logs to the repository.
    /// </summary>
    public async Task LogUpload()
    {
        Console.WriteLine("Log upload initiated.");
        try
        {
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { "public.plain-text" } },
                { DevicePlatform.Android, new[] { "text/plain" } },
                { DevicePlatform.WinUI, new[] { ".txt" } },
                { DevicePlatform.MacCatalyst, new[] { "public.plain-text" } }
            });
            var options = new PickOptions
            {
                PickerTitle = "Select log file to upload",
                FileTypes = customFileType
            };
            var result = await FilePicker.Default.PickAsync();
            if (result == null)
            {
                Console.WriteLine("No file selected for upload.");
                return;
            }
            FileName = result.FileName;
            FilePath = result.FullPath;
            await ProcessLogEntryAsync(FilePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"File pick error: {ex.Message}");
            return;
        }
    }

    private async Task ProcessLogEntryAsync(string filePath)
    {
        Console.WriteLine($"Processing log from: {filePath}");
        try
        {
            // Example: create a new upload and add entries
            var upload = new LogUpload
            {
                Id = Guid.NewGuid(),
                FileName = Path.GetFileName(filePath),
                CreatedUtc = DateTimeOffset.UtcNow
            };
            await _repository.CreateUploadAsync(upload);
            // Parse file and add entries (stub)
            var lines = await File.ReadAllLinesAsync(filePath);
            var entries = lines.Select(line => new LogEntry
            {
                FileName = upload.FileName,
                Message = line,
                CreatedAt = DateTime.UtcNow
            });
            await _repository.AddEntriesAsync(upload.Id, entries);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing log entry: {ex.Message}");
        }
        LoadUploads();
    }


    [RelayCommand]
    private async Task GenerateSampleLogsAsync()
    {
        for (int i = 1; i <= 50; i++)
        {
            var upload = new LogUpload
            {
                Id = Guid.NewGuid(),
                FileName = $"sample_log_{i}.txt",
                CreatedUtc = DateTimeOffset.UtcNow.AddMinutes(-i)
            };
            await _repository.CreateUploadAsync(upload);
            int detailCount = new Random().Next(5, 11);
            var entries = Enumerable.Range(1, detailCount).Select(j => new LogEntry
            {
                FileName = upload.FileName,
                Message = $"Sample log message {j} for upload {i}",
                CreatedAt = DateTime.UtcNow.AddMinutes(-i).AddSeconds(j)
            });
            await _repository.AddEntriesAsync(upload.Id, entries);
        }
        LoadUploads();
    }

    [RelayCommand]
    private void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(FilterText))
        {
            LoadUploads();
        }
        else
        {
            var uploads = _repository.GetUploadsAsync().Result
                .Where(u => u.FileName.Contains(FilterText, StringComparison.OrdinalIgnoreCase))
                .ToList();
            Uploads = new ObservableCollection<LogUpload>(uploads);
        }
    }

    [RelayCommand]
    private void UploadSelected()
    {
        if (SelectedUpload == null)
        {
            Console.WriteLine("No upload selected.");
            return;
        }
        Console.WriteLine($"Upload selected: {SelectedUpload.FileName}");
        var entries = _repository.GetEntriesAsync(SelectedUpload.Id).Result;
        SelectedUploadEntries = new ObservableCollection<LogEntry>(entries);
        Console.WriteLine($"Loaded {SelectedUploadEntries.Count} entries for selected upload.");
    }


}