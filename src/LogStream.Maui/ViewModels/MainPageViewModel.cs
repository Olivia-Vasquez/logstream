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
    private LogsDatabase _database;

    private ILogParser _logParser; 
    [ObservableProperty]
    private ObservableCollection<Item>? _items;
    [ObservableProperty]
    private ObservableCollection<ItemDetail>? _itemDetails;

    [ObservableProperty]
    private Item? _selectedItem;

    [ObservableProperty]
    private ObservableCollection<ItemDetail>? _selectedItemDetails;

    [ObservableProperty]
    private string? _fileName;

    [ObservableProperty]
    private string? _filePath;

    [ObservableProperty]
    private string? _filterText;

    public MainPageViewModel(LogsDatabase database)
    {
        // Inject database service
        _database = database;
        LoadLogs();
        _logParser = new FileLogParser(_database); // Initialize the FileLogParser

    }


    private void LoadLogs()
    {
        // Placeholder for loading logs from the database
        Console.WriteLine("Loading logs from the database...");

        Items = new ObservableCollection<Item>(_database.GetItemsAsync().Result); // For simplicity, using .Result to wait for the async method
        Console.WriteLine($"Loaded {Items.Count} items from the database.");
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
            // Create custom filepicker file type
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


        try
        {
            // Use ILogParser with FileLogParser implementation to check if the file can be parsed
            if (!_logParser.CanParse(filePath))
            {
                Console.WriteLine("Selected file cannot be parsed by the log parser.");
                return;
            }
            // Use ILogParser with FileLogParser implementation to parse the log file and create Item and ItemDetail entries in the database
            _logParser.Parse(filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing log entry: {ex.Message}");
        }

        LoadLogs(); // Refresh the log entries after processing
    }


    [RelayCommand]
    private void GenerateSampleLogs()
    {
        // Generate 50 sample items with unique names names and 200 item details for testing

        for (int i = 1; i <= 50; i++)
        {
            var item = new Item
            {
                FileName = $"sample_log_{i}.txt",
                CreatedAt = DateTime.UtcNow.AddMinutes(-i), // Stagger creation times
                DetailCount = 0
            };
            _database.InsertItemAsync(item).Wait(); // Wait for the async operation to complete

            for (int j = 1; j <= 4; j++)
            {
                var detail = new ItemDetail
                {
                    ItemId = item.Id,
                    LineNumber = j,
                    Timestamp = DateTime.UtcNow.AddMinutes(-i).AddSeconds(j), // Stagger timestamps
                    Level = j % 2 == 0 ? "INFO" : "ERROR",
                    Message = $"Sample log message {j} for item {i}",
                    Raw = $"Raw log line {j} for item {i}"
                };
                _database.InsertItemDetailAsync(detail).Wait(); // Wait for the async operation to complete
                item.DetailCount++;
            }

            // Update the item with the correct DetailCount
            _database.InsertItemAsync(item).Wait(); // Wait for the async operation to complete
        }

        // Generate 5-10 sample item details for each item to test large log files
        for (int i = 1; i <= 50; i++)
        {
            var item = new Item
            {
                FileName = $"sample_log_{i}.txt",
                CreatedAt = DateTime.UtcNow.AddMinutes(-i), // Stagger creation times
                DetailCount = 0
            };
            _database.InsertItemAsync(item).Wait(); // Wait for the async operation to complete

            int detailCount = new Random().Next(5, 11); // Generate 5-10 details
            for (int j = 1; j <= detailCount; j++)
            {
                var detail = new ItemDetail
                {
                    ItemId = item.Id,
                    LineNumber = j,
                    Timestamp = DateTime.UtcNow.AddMinutes(-i).AddSeconds(j), // Stagger timestamps
                    Level = j % 2 == 0 ? "INFO" : "ERROR",
                    Message = $"Sample log message {j} for item {i}",
                    Raw = $"Raw log line {j} for item {i}"
                };
                _database.InsertItemDetailAsync(detail).Wait(); // Wait for the async operation to complete
                item.DetailCount++;
            }

            // Update the item with the correct DetailCount
            _database.InsertItemAsync(item).Wait(); // Wait for the async operation to complete
        }

        LoadLogs(); // Refresh the log entries after generating sample data
    }

    [RelayCommand]
    private void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(FilterText))
        {
            // If filter is empty, show all items
            Items = new ObservableCollection<Item>(_database.GetItemsAsync().Result); // For simplicity, using .Result to wait for the async method
        }
        else
        {
            // Filter items based on FileName containing the filter text
            var filteredItems = _database.GetItemsAsync().Result
                .Where(item => item.FileName.Contains(FilterText, StringComparison.OrdinalIgnoreCase))
                .ToList();

            Items = new ObservableCollection<Item>(filteredItems);
        }
    }

    [RelayCommand]
    private void ItemSelected()
    {
        if (SelectedItem == null)
        {
            Console.WriteLine("No item selected.");
            return;
        }

        Console.WriteLine($"Item selected: {SelectedItem.FileName}");
        // Load details for the selected item
        SelectedItemDetails = new ObservableCollection<ItemDetail>(_database.GetItemDetailsAsync(SelectedItem.Id).Result); // For simplicity, using .Result to wait for the async method
        Console.WriteLine($"Loaded {SelectedItemDetails.Count} details for selected item.");
    }


}