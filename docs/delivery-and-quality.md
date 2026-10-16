# Delivery and quality handoff

## Current delivery posture

This repository has no CI workflow, release process, or automated test project. It targets Windows and requires Visual Studio/MSBuild support for .NET Framework 4.8.1 plus a compatible SQL Server environment.

## Suggested exploratory verification

1. Restore NuGet packages from `packages.config`.
2. Build the solution in Visual Studio on Windows.
3. Run only against a disposable local SQL Server instance with the required schema.
4. Verify currency creation/edit/delete and a representative conversion.
5. If exercising the rate helper, confirm the provider's current API requirements and use non-production data.

## Known gaps

- No database bootstrap or migration scripts are present.
- The application configuration is tied to a developer-local SQL Server instance.
- No automated tests, CI checks, deployment target, or support owner are documented.
- Endpoint behavior and data-source terms may have changed since the original exercise.

## Lightweight work management

For any follow-up, record a small issue with: objective, in/out of scope, acceptance criteria, validation evidence, risk/rollback, and owner decision. Do not expand the project into a production currency service without an approved modernization charter.

## Archive decision gate

Retain the repository as a learning reference or archive it after confirming that no portfolio narrative depends on it. Rename it only if the owner chooses to actively retain and modernize it.
