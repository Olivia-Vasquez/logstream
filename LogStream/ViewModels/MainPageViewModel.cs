using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LogStream.Models;
using LogStream.Services;
using Microsoft.Maui.Storage;

namespace LogStream.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    private LogsDatabase _database;
    [ObservableProperty]
    private ObservableCollection<LogEntry> _logEntries;

    [ObservableProperty]
    private string? _fileName;

    [ObservableProperty]
    private string? _filePath;


    public MainPageViewModel(LogsDatabase database)
    {
        // Inject database service
        _database = database;
        LoadLogs();

    }

    private void LoadLogs()
    {
        // Placeholder for loading logs from the database
        Console.WriteLine("Loading logs from the database...");

        LogEntries = new ObservableCollection<LogEntry>(_database.GetLogsAsync().Result); // For simplicity, using .Result to wait for the async method
    }

    [RelayCommand]
    private void CreateSampleLog()
    {
        // Placeholder for creating a log entry
        Console.WriteLine("CreateSampleLog command executed.");
        
        // Create a sample txt file in the app's data directory for testing 
        var sampleFilePath = Path.Combine(FileSystem.AppDataDirectory, "sample_log.txt");

        File.WriteAllText(sampleFilePath, "This is a sample log entry.");
        Console.WriteLine($"Sample log file created at: {sampleFilePath}");
    }

    [RelayCommand]
    /// <summary>
    /// Uploads the logs to the server.
    /// </summary>
    public async Task LogUpload()
    {
        // Checkpoint: Log upload initiated
        Console.WriteLine("Log upload initiated.");

        try
        {
            // Create custome filepicker file type
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { "public.plain-text" } }, // UTI for .txt files
                { DevicePlatform.Android, new[] { "text/plain" } }, // MIME type for .txt files
                { DevicePlatform.WinUI, new[] { ".txt" } }, // Extension for .txt files
                { DevicePlatform.MacCatalyst, new[] { "public.plain-text" } } // UTI for .txt files
            });
            // Set up picker options
            var options = new PickOptions
            {
                PickerTitle = "Select log file to upload",
                FileTypes = customFileType // You can customize this to specific extensions if needed
            };

            var result = await FilePicker.Default.PickAsync();
            if (result == null)
            {
                Console.WriteLine("No file selected for upload.");
                return;
            }
            else
            {
                Console.WriteLine("File selected for upload. Processing...");
                FileName = result.FileName;
                FilePath = result.FullPath;
                ProcessLogEntry(FilePath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"File pick error: {ex.Message}");
            return;
        }
        


    }

    private void ProcessLogEntry(string filePath)
    {
        // Placeholder for actual processing logic
        Console.WriteLine($"Processing log from: {filePath}");
        // Here you would read the file, create a LogEntry, and save it to the database

        try
        {
            var logEntry = new LogEntry
            {
                FileName = FileName ?? "Error: Unknown File",
                Message = $"Log file {FileName ?? "Unknown File"} uploaded successfully.",
                CreatedAt = DateTime.UtcNow
            };

            // Save to database
            var saveTask = _database.SaveLogAsync(logEntry);
            saveTask.Wait(); // Wait for the async operation to complete

            Console.WriteLine("Log entry saved to database.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing log entry: {ex.Message}");
        }

        LoadLogs(); // Refresh the log entries after processing
    }


}