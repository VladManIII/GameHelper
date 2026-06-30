# GameHelper Main Architecture

This document describes the main architecture of the GameHelper application, targeting .NET 10.

## UI Framework
- **WinUI** is used for the desktop interface, following the MVVM (Model-View-ViewModel) pattern for separation of concerns and testability.

## Project Structure
- **Main Project:** Contains core application logic, organized into:
  - `Views`
  - `ViewModels`
  - `Models`
  - `Helpers`
  - `Services`
- **Key Hooks Project:** Added as a dependency to the main project, responsible for low-level key hook logic.

## Dependency Injection
- All services (e.g., database, hook service) are initialized at application startup. The app features a dock icon for quick access.

## Data Flow
- **View/ViewModel:** Data binding is used for communication between Views and ViewModels.
- **Database:** A singleton service manages all CRUD operations with a local LiteDB database.

## Macro Execution Engine
- A dedicated service subscribes to key events and executes macros asynchronously using async/await.

## Event Aggregation/Messaging
- Event subscription is used to trigger macro execution in response to key events.

## Settings Management
- Application settings are stored as a separate entity in the database, including autorun state, key hook toggles, and other preferences.

## Error Handling & Logging
- Errors are logged to the console. Critical errors may also be displayed as pop-up notifications.

## Testing Strategy
- Most code is covered by unit tests to ensure reliability and maintainability.

## Threading/Concurrency
- Asynchronous programming (async/await) is used to keep the UI responsive during background operations (e.g., macro execution, database access).

## Extensibility
- The architecture supports extension by adding new classes and interfaces.
