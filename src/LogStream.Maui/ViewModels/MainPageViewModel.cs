using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using CommunityToolkit.Maui;

using LogStream.Core.Models;
using LogStream.Core.Abstractions;
using LogStream.Core.Parsing;

using LogStream.Maui.Services;
using CommunityToolkit.Maui.Extensions;
using LogStream.Maui.Views;


namespace LogStream.Maui.ViewModels;
public partial class MainPageViewModel : ObservableObject
{
    // Services
    private ILogRepository? _repository;
    private IPopupService? _popupService;
    private IThemeService? _themeService;
    // private ILogParser? _logParser;

    // Observable properties
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

    public MainPageViewModel(ILogRepository repository, IPopupService popupService, IThemeService themeService)
    {
        InitServices(repository, popupService, themeService);
        // Fire-and-forget: do NOT block the UI thread with .Result
        _ = LoadUploadsAsync();
    }

    private void InitServices(ILogRepository repository, IPopupService popupService, IThemeService themeService)
    {
        Console.WriteLine("Initializing MainPageViewModel services...");
        
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _popupService = popupService ?? throw new ArgumentNullException(nameof(popupService));
        _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
    }

    [RelayCommand]
    public async Task OpenSettings()
    {
        if(_themeService == null || _popupService == null)
        {
            Console.WriteLine("Cannot open settings: services not initialized.");
            return;
        }

        await Shell.Current.ShowPopupAsync(new SettingsPopup(new SettingsViewModel(themeService: _themeService)));
    }

    private async Task LoadUploadsAsync()
    {
        if(_repository == null)
        {
            Console.WriteLine("Cannot load uploads: repository not initialized.");
            return;
        }

        // Loads uploads from repository
        Console.WriteLine("Loading uploads from repository...");
        var uploads = await _repository.GetUploadsAsync();
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
        if (_repository == null) return;

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
        await LoadUploadsAsync();
    }


    [RelayCommand]
    private async Task GenerateSampleLogsAsync()
    {
        if (_repository == null) return;
        
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
        await LoadUploadsAsync();
    }

    [RelayCommand]
    private async Task ApplyFilterAsync()
    {
        if (string.IsNullOrWhiteSpace(FilterText))
        {
            await LoadUploadsAsync();
        }
        else
        {
            if (_repository == null) return;
            var uploads = await _repository.GetUploadsAsync();
            var filtered = uploads
                .Where(u => u.FileName.Contains(FilterText, StringComparison.OrdinalIgnoreCase))
                .ToList();
            Uploads = new ObservableCollection<LogUpload>(filtered);
        }
    }

    [RelayCommand]
    private async Task UploadSelectedAsync()
    {
        if(_repository == null) return;

        if (SelectedUpload == null)
        {
            Console.WriteLine("No upload selected.");
            return;
        }
        Console.WriteLine($"Upload selected: {SelectedUpload.FileName}");
        var entries = await _repository.GetEntriesAsync(SelectedUpload.Id);
        SelectedUploadEntries = new ObservableCollection<LogEntry>(entries);
        Console.WriteLine($"Loaded {SelectedUploadEntries.Count} entries for selected upload.");
    }


}