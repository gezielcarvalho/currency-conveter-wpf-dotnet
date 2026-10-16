# Architecture and local setup

## Components

| Component | Responsibility |
| --- | --- |
| `MainWindow.xaml` and code-behind | WPF screens, interaction handlers, validation, conversion flow, and initial currency seeding. |
| `Models/` | Currency and rate-transfer types. |
| `CurrencyConverterDbDataSet.*` | Generated typed-dataset and SQL Server table-adapter code. |
| `MainWindowHelpers.cs` | HTTP retrieval helper for rate JSON. |
| `App.config` | .NET Framework startup settings and a developer-local SQL Server connection-string entry. |

## Data and integration flow

1. The application reads its SQL Server connection setting at startup.
2. Startup attempts to seed common currencies when records are absent.
3. User actions read and update local currency/conversion records through ADO.NET.
4. The rate helper can request JSON from the configured public endpoint; its response is deserialized with Newtonsoft.Json.

## Local setup limitations

The repository contains no database creation scripts or portable local environment. To explore it safely, use a disposable SQL Server instance, provision the expected schema from the generated dataset/query expectations, and point a local, uncommitted configuration copy at that instance. Do not treat the checked-in developer-machine connection setting as a reusable environment configuration.

## Security and reliability notes

- The endpoint configuration does not demonstrate an application key or a documented provider plan; validate its current requirements before use.
- Network and database failures are surfaced to the desktop user but lack centralized logging and retry policy.
- Currency values use floating-point numeric values in existing code; modernization should define decimal precision and rounding rules before financial use.
