# Project charter

## Problem and learning objective

Build a desktop interface that converts between stored currencies and lets an operator maintain the local currency catalogue. The project is a learning artifact rather than a maintained product.

## Scope retained in the repository

- WPF/XAML user interface and validation flows.
- SQL Server-backed currency and conversion records.
- Initial currency seeding and conversion calculations.
- A JSON exchange-rate retrieval helper.

## Out of scope

- A hosted or distributed production service.
- Authentication, multi-user authorization, or audit-grade financial controls.
- A reproducible database schema/bootstrap workflow.
- A supported deployment, CI pipeline, or test suite.

## Stakeholders and ownership

The repository is maintained as part of the personal engineering archive. Any modernization, publication, or deletion decision requires owner approval.

## Success criteria for a future modernization

1. Migrate to a supported modern .NET desktop target or document a justified alternative.
2. Externalize the local database configuration and provide a reproducible schema/seed path.
3. Use a supported, authenticated rate provider with rate-source attribution and failure handling.
4. Add automated coverage for conversion and validation rules.
5. Define currency precision, rounding, update cadence, and operational support expectations.
