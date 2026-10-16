# Current-state documentation

## Purpose

This repository is a historical WPF learning project for currency conversion and local currency-record management. It demonstrates XAML desktop UI work, ADO.NET/SQL Server access, input validation, and consuming a JSON HTTP endpoint.

## Current state

- Target: .NET Framework 4.8.1 Windows desktop application.
- Interface: WPF `MainWindow` with conversion and currency-management interactions.
- Persistence: SQL Server tables accessed through ADO.NET and generated typed dataset adapters.
- Rates: seeded local values; a helper targets the Open Exchange Rates JSON endpoint.
- Dependencies: Newtonsoft.Json and FontAwesome.WPF through `packages.config`.
- Tests and CI: none were found.

## Boundaries

This is not production financial software. Exchange-rate freshness, source licensing/availability, monetary rounding rules, observability, and deployment are not established here. The included SQL Server connection string names a developer-local instance and must be replaced with an environment-specific configuration before anyone runs the application.

## Decision

Keep as a reference for early WPF, SQL Server, and desktop-application work; exclude it from the primary portfolio showcase unless it is deliberately modernized. The repository name remains unchanged while this archive decision is pending.

See the [project charter](project-charter.md), [architecture](architecture.md), and [delivery handoff](delivery-and-quality.md).
