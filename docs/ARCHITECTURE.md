# LogStream Architecture

## Overview

LogStream is structured to support multiple frontends while maintaining a shared core logic. The project will be organized into the following main folders:

- `LogStream.Core`: Contains the core application logic, models, services, and shared utilities.
- `LogStream.Web`: Hosts the web frontend, interfacing with `LogStream.Core`.
- `LogStream.Maui`: Hosts the MAUI frontend for cross-platform mobile and desktop support.

## Folder Structure

```
/LogStream.Core
    └── Core business logic, models, services
/LogStream.Web
    └── Web-specific UI and integration
/LogStream.Maui
    └── MAUI-specific UI and integration
```

## Design Principles

- **Separation of Concerns**: Core logic is isolated from frontend implementations.
- **Reusability**: Shared code in `LogStream.Core` is reused by both frontends.
- **Extensibility**: New frontends can be added with minimal changes to core logic.

## Future Enhancements

- Add API layer in `LogStream.Core` for frontend communication.
- Implement dependency injection for modularity.
- Expand documentation as architecture evolves.
