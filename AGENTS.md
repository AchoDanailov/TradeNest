# TradeNest — Agent Guide

## Stack
- .NET 8, ASP.NET Core MVC, EF Core 8, SQL Server 2022+, NUnit + Moq
- Razor Views + Bootstrap 5; TypeScript (lit-html, DOMPurify, sweetalert2) compiled to `wwwroot/js`
- Dev server: http://localhost:5188 (http profile in `launchSettings.json`)

## Architecture (N-tier)
```
TradeNest.Web/          — Controllers (MVC + `*ApiController`), Areas, Views, presentation-layer mappers
TradeNest.Web.Models/   — ViewModels & DTOs for the web boundary
TradeNest.Web.Infrastructure/ — Filters (WebApiExceptionFilter), DI registration extensions
TradeNest.Services.Core/      — Business logic, service interfaces & Mapperly mappers
TradeNest.Services.Models/    — Service-layer DTOs
TradeNest.Data/               — EF Core DbContext, migrations, repositories, seeders, QueryOptions
TradeNest.Data.Models/        — Entity POCOs
TradeNest.Data.Common/        — Data-layer shared abstractions
TradeNest.GCommon/            — Constants (EntityValidationConstants, ApplicationConstants), exceptions, error messages
```

## Commands
```bash
# First-time restore (order matters: npm install before build)
dotnet tool restore && dotnet restore && cd src/TradeNest.Web && npm install && cd ../..

# Build — NOTE: runs `npx tsc` via MSBuild target (.csproj:27-29), so Node + node_modules are required
dotnet build

# Run (dev server on http://localhost:5188)
dotnet run --project src/TradeNest.Web

# Apply migrations (dotnet-ef pinned in .config/dotnet-tools.json)
dotnet ef database update --project src/TradeNest.Data --startup-project src/TradeNest.Web

# Test all / single project
dotnet test
dotnet test tests/unit/TradeNest.Services.Tests
dotnet test tests/unit/TradeNest.Data.Tests
dotnet test tests/integration/TradeNest.Data.IntegrationTests

# TypeScript only (skip the full dotnet build)
npx tsc -p src/TradeNest.Web/tsconfig.json
```

## DI Registration (assembly scanning — naming convention is mandatory)
Auto-wired in `TradeNest.Web.Infrastructure.Extensions/WebApplicationBuilderExtensions.cs`, invoked from `Program.cs:52-61`. Follow the naming convention; no manual wiring:
- `I*Service` → `*Service` (scoped)
- `I*Repository` → `*Repository` (scoped); generic interfaces (e.g. `IReadRepository<T>`) are skipped
- `I*Mapper` → `*Mapper` (singleton)
- `I*Seeder` → `*Seeder` (scoped, dev-only); interfaces whose name contains "Entity" (e.g. `IEntitySeeder`) are skipped

## Mapping (Mapperly)
Two layers, each interface + partial class, both `[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]`:
- `Services.Core/Mappers/` — Entity ↔ Service DTO
- `Web/Mappers/` — Service DTO ↔ ViewModel/FormModel

Custom mappings use `[MapProperty]`, `[MapPropertyFromSource]`, `[MapperIgnoreTarget]`.

## Areas & Routing
Area route (`Program.cs:89-91`) declared before the default route (`Program.cs:92-94`):
| Area       | Route prefix           |
|------------|------------------------|
| Admin      | `/Admin/{controller}`  |
| Identity   | `/Identity/{controller}` |
| MyNest     | `/MyNest/{controller}` |

## Web API conventions
- `BaseApiController` (`[Route("/api/v1")]`, `[Authorize]`, `[ApiController]`) is the base for all `*ApiController` endpoints (in `Controllers/`, not Areas).
- Exceptions are centralized in `WebApiExceptionFilter` (registered `Program.cs:44`, applied via `[ServiceFilter<...>]` on the base): `ResourceNotFoundException`→404, `UnauthorizedOperationException`→403, `ArgumentException`/`InvalidOperationException`→400, else 500 (JSON `{ Status, Error }`). Throw GCommon exceptions from services; don't catch-and-return in controllers.
- The JS antiforgery token comes from the products list response `MetaData.XsrfToken` (`ProductsApiController.GetProductsDataAsync`) and must be sent on every state-changing AJAX call.

## Security
- Antiforgery header `X-XSRF-TOKEN` (`Program.cs:38-39`); every fetch/AJAX write must include it. MVC forms are validated automatically via `[AutoValidateAntiforgeryToken]` on base controllers.
- Sanitize user-provided content rendered via lit-html with `DOMPurify`.
- Identity cookie configured via `CookieAuthOptions` in `appsettings.json`.

## Database
- Connection string: `TradeNest:ConnectionString`, else `ConnectionStrings:DefaultConnection` (`Program.cs:23-25`). Dev overrides via User Secrets — `appsettings.Development.json` is gitignored.
- Seeding auto-runs in Development only (`app.UseSeeding()`, `Program.cs:76`).
- Test logins: `User1`–`User3` / `Password1`–`Password3`, `Admin1` / `Admin1Password`.

## Testing
- `dotnet test` needs no SQL Server: the integration tests use EF Core InMemory (`TradeNestTestDb`).
- Repository queries use fluent `QueryOptions<T>` (`SetFilter`, `WithRelated`, `AddOrderAsc/Desc`, `WithPagination`, `AsReadOnly`).
- `tests/e2e/` is empty — Playwright setup pending.

## CI (`.github/workflows/ci.yml`)
Runs on push to **any branch** + PRs to `main`: `npm ci --prefix src/TradeNest.Web` → `dotnet restore` → `dotnet build --no-restore` → `dotnet test --no-build`.
