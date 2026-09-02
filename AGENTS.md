# TradeNest — Agent Guide

## Stack
- .NET 8, ASP.NET Core MVC, EF Core 8, SQL Server 2022+, NUnit + Moq
- Razor Views + Bootstrap 5; TypeScript (lit-html, DOMPurify, sweetalert2) compiled to `wwwroot/js`
- Dev server: http://localhost:5188 (`launchSettings.json` "http" profile)

## Architecture (N-tier)
```
TradeNest.Web/              — Controllers (MVC + *ApiController), Areas, Views, presentation mappers
TradeNest.Web.Models/       — ViewModels & DTOs for web boundary
TradeNest.Web.Infrastructure/ — Filters (WebApiExceptionFilter), DI extensions
TradeNest.Services.Core/    — Business logic, service interfaces & Mapperly mappers
TradeNest.Services.Models/  — Service-layer DTOs
TradeNest.Data/             — EF Core DbContext, migrations, repositories, seeders, QueryOptions
TradeNest.Data.Models/      — Entity POCOs
TradeNest.Data.Common/      — Data-layer shared abstractions
TradeNest.GCommon/          — Constants, exceptions, error messages
```

## Commands
```bash
# First-time restore (order matters: npm install before build)
dotnet tool restore && dotnet restore && cd src/TradeNest.Web && npm install && cd ../..

# Build — TS compiles via Microsoft.TypeScript.MSBuild (bundled TS 6.0.3); output to wwwroot/js (gitignored)
dotnet build

# Run (dev server on http://localhost:5188)
dotnet run --project src/TradeNest.Web

# Migrations (dotnet-ef pinned in .config/dotnet-tools.json)
dotnet ef database update --project src/TradeNest.Data --startup-project src/TradeNest.Web

# Test
dotnet test
dotnet test tests/unit/TradeNest.Services.Tests
dotnet test tests/unit/TradeNest.Data.Tests
dotnet test tests/integration/TradeNest.Data.IntegrationTests
```

## DI Registration (assembly scanning — naming convention mandatory)
Auto-wired in `WebApplicationBuilderExtensions.cs` (invoked `Program.cs:52-61`). No manual wiring:
- `I*Service` → `*Service` (scoped)
- `I*Repository` → `*Repository` (scoped); generic interfaces skipped
- `I*Mapper` → `*Mapper` (singleton)
- `I*Seeder` → `*Seeder` (scoped, dev-only); interfaces containing "Entity" skipped

## Mapping (Mapperly)
Two layers, interface + partial class, both `[Mapper(RequiredMappingStrategy = Target)]`:
- `Services.Core/Mappers/` — Entity ↔ Service DTO
- `Web/Mappers/` — Service DTO ↔ ViewModel/FormModel
Custom: `[MapProperty]`, `[MapPropertyFromSource]`, `[MapperIgnoreTarget]`

## Areas & Routing
Area route (`Program.cs:89-91`) before default (`Program.cs:92-94`):
| Area    | Route prefix          |
|---------|-----------------------|
| Admin   | `/Admin/{controller}` |
| Identity| `/Identity/{controller}` |
| MyNest  | `/MyNest/{controller}` |

## Web API Conventions
- `BaseApiController`: `[Route("/api/v1")]`, `[Authorize]`, `[ApiController]`, `[ServiceFilter<WebApiExceptionFilter>]`, `[AutoValidateAntiforgeryToken]`
- Exceptions in `WebApiExceptionFilter`: `ResourceNotFoundException`→404, `UnauthorizedOperationException`→403, `ArgumentException`/`InvalidOperationException`→400, else 500 (JSON `{ Status, Error }`). Throw GCommon exceptions from services; don't catch-and-return in controllers.
- XSRF token from `ProductsApiController.GetProductsDataAsync` response `MetaData.XsrfToken` — required on every state-changing AJAX call.

## Security
- Antiforgery header `X-XSRF-TOKEN` (`Program.cs:38-39`); every fetch/AJAX write must include it. MVC forms auto-validated via `[AutoValidateAntiforgeryToken]`.
- Sanitize user content rendered via lit-html with `DOMPurify`.
- Identity cookie via `CookieAuthOptions` in `appsettings.json`.

## Database
- Connection: `TradeNest:ConnectionString` → `ConnectionStrings:DefaultConnection` (`Program.cs:23-25`). Dev overrides via User Secrets (`appsettings.Development.json` gitignored).
- Seeding auto-runs in Development only (`app.UseSeeding()`, `Program.cs:76`).
- Test logins: `User1`-`User3` / `Password1`-`Password3`, `Admin1` / `Admin1Password`.

## Testing
- `dotnet test` needs no SQL Server: integration tests use EF Core InMemory (`TradeNestTestDb`).
- Repository queries use fluent `QueryOptions<T>` (`SetFilter`, `WithRelated`, `AddOrderAsc/Desc`, `WithPagination`, `AsReadOnly`).
- `tests/e2e/` empty — Playwright pending.

## CI (`.github/workflows/ci.yml`)
Push to any branch + PRs to `main`: `npm ci --prefix src/TradeNest.Web` → `dotnet restore` → `dotnet build --no-restore` → `dotnet test --no-build`
