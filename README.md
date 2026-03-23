# LogStream
Cross-platform desktop and mobile app for ingesting, indexing and browsing structured log files.

## Current Status

LogStream is actively in development. The MAUI app (macOS, iOS, Android, Windows) is the primary frontend and is functional for local log ingestion and browsing. The web frontend is planned but not yet started.

## Features

- **Log Upload:** Pick `.log` files from disk and ingest them into a local SQLite database.
- **Log Browsing:** Browse uploaded log files and page through their entries.
- **Search & Filter:** Filter uploads and entries by keyword.
- **Persistent Storage:** All uploads and log entries survive app restarts via SQLite.
- **Theme Support:** Light, dark, and system theme modes with live switching.
- **Shared Core:** Business logic and data models live in `LogStream.Core`, shared across frontends.

## Project Structure

```
LogStream.sln
├── src/
│   ├── LogStream.Core/        # Models, repository abstractions, log parser
│   └── LogStream.Maui/        # .NET MAUI cross-platform app
│       ├── Platforms/         # MacCatalyst, iOS, Android, Windows entry points
│       ├── Services/          # SQLite database, theme service
│       ├── ViewModels/        # MainPage, Settings (CommunityToolkit.Mvvm)
│       └── Views/             # MainPage, SettingsPopup
└── tests/
    ├── LogStream.Core.Tests/
    └── LogStream.Maui.Tests/
```

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- .NET MAUI workload: `dotnet workload install maui`
- VS Code with the [.NET MAUI extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.dotnet-maui)

### Build

```bash
dotnet build src/LogStream.Maui/LogStream.Maui.csproj -f net10.0-maccatalyst
```

### Run

Open the project in VS Code and launch via the `.NET MAUI` debug configuration, or use the MAUI extension's run button with `net10.0-maccatalyst` selected as the target framework.

## Roadmap

- [ ] `LogStream.Web` — Blazor or ASP.NET web frontend backed by the same `LogStream.Core`
- [ ] Structured log parsing improvements (timestamps, severity levels)
- [ ] Export / share uploaded log data
- [ ] API layer in `LogStream.Core` for frontend communication

## Contributing

Contributions are welcome! Please open issues or submit pull requests for bug fixes, features, or documentation improvements.

## License

This project is licensed under the MIT License.

## Contact

For questions or support, please contact [Olivia Vasquez](mailto:olivia@fakemail.com).