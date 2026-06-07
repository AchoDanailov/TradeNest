# TradeNest — Agent Guide

## Stack
- .NET 8, ASP.NET Core MVC, EF Core 8, SQL Server 2022+
- Razor Views + Bootstrap 5 + TypeScript + lit-html
- Tests: NUnit + Moq + coverlet

## Architecture (N-tier)
```
TradeNest.Web/          — Controllers, Views, Areas, WebApi, presentation-layer mappers
TradeNest.Web.Models/   — ViewModels & DTOs for the web boundary
TradeNest.Web.Infrastructure/ — Filters, Middleware, DI registration extensions
TradeNest.Services.Core/      — Business logic, service interfaces & Mapperly mappers
TradeNest.Services.Models/    — Service-layer DTOs
TradeNest.Data/               — EF Core DbContext, migrations, repositories, seeders
TradeNest.Data.Models/        — Entity POCOs
TradeNest.Data.Common/        — Data-layer shared abstractions
TradeNest.GCommon/            — Cross-cutting (constants, exceptions, validation)
```

## Key Commands

```bash
# Restore everything
dotnet tool restore && dotnet restore && cd src/TradeNest.Web && npm install && cd ../..

# Build
dotnet build

# Run (dev server on http://localhost:5188)
dotnet run --project src/TradeNest.Web

# Apply migrations
dotnet ef database update --project src/TradeNest.Data --startup-project src/TradeNest.Web

# Test all
dotnet test

# Test single project
dotnet test tests/unit/TradeNest.Services.Tests
dotnet test tests/unit/TradeNest.Data.Tests
dotnet test tests/integration/TradeNest.Data.IntegrationTests

# TypeScript compile (runs automatically on `dotnet build` via MSBuild target)
npx tsc -p src/TradeNest.Web/tsconfig.json
```

## DI Registration Convention

Services, repositories, mappers, and seeders are **auto-registered via assembly scanning** in `TradeNest.Web.Infrastructure.Extensions/WebApplicationBuilderExtensions.cs`. Naming convention is mandatory:
- Interface `I*Service` → class `*Service` (scoped)
- Interface `I*Repository` → class `*Repository` (scoped)
- Interface `I*Mapper` → class `*Mapper` (singleton)
- Interface `I*Seeder` → class `*Seeder` (scoped, dev-only)

Registration is invoked in `Program.cs:52-61`. Add new types by following the naming convention — no manual DI wiring needed.

## Mapping (Mapperly)

Two mapper layers, each with interface + partial class:
- `Services.Core/Mappers/` — Entity → Service DTO (and reverse)
- `Web/Mappers/` — Service DTO → ViewModel / FormModel (and reverse)

Both use `[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]`. Custom mappings use `[MapProperty]`, `[MapPropertyFromSource]`, and `[MapperIgnoreTarget]`.

## Areas & Routing

| Area       | Route prefix           |
|------------|------------------------|
| Admin      | `/Admin/{controller}`  |
| Identity   | `/Identity/{controller}` |
| MyNest     | `/MyNest/{controller}` |

Default route: `{controller=Home}/{action=Index}/{id?}`. Area route defined first in `Program.cs:89-94`.

## Security

- Antiforgery header name: `X-XSRF-TOKEN` (configured `Program.cs:38-39`)
- All AJAX/fetch requests must include this header
- Use `DOMPurify` when rendering user-provided content via lit-html (package dependency)
- Identity cookie configured via `CookieAuthOptions` section in `appsettings.json`

## Database

- Connection string resolved from `TradeNest:ConnectionString` or `ConnectionStrings:DefaultConnection` (in that order, `Program.cs:23-25`)
- Dev seeding auto-runs when `ASPNETCORE_ENVIRONMENT=Development` (via `app.UseSeeding()`)
- Default test users: `User1`–`User3` / `Password1`–`Password3`, `Admin1` / `Admin1Password`

## TypeScript

- Source: `src/TradeNest.Web/FrontEndScripts/`
- Output: `wwwroot/js/` (via `tsconfig.json`)
- Compiled automatically on `dotnet build` by MSBuild target in `.csproj:27-29`
- If you only change TS, run `npx tsc -p src/TradeNest.Web/tsconfig.json` or `dotnet build`

## CI (`.github/workflows/ci.yml`)

Push/PR to `main`: `npm ci` → `dotnet restore` → `dotnet build --no-restore` → `dotnet test --no-build`

## Conventions

- `Nullable` enabled, `ImplicitUsings` enabled
- Services depend on repository interfaces, not on EF Core directly
- Repository pattern uses fluent `QueryOptions` for filtering and includes
- Validation constants live in `TradeNest.GCommon/EntityValidationConstants.cs`
- `appsettings.Development.json` is gitignored — use `appsettings.json` and/or User Secrets for local overrides
- E2E test directory (`tests/e2e/`) is empty — Playwright setup pending
- Long-term: plans to adopt `TryAtSoftware/CleanTests` for some tests
