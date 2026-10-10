# Training module starter

A trimmed copy of the NAFADH Jobseeker platform for building a new module with the same structure, patterns and database.

It contains:

- **Login and registration**: staff login, jobseeker login, PKI callback, jobseeker registration, email verification, onboarding gate.
- **User management (Identity & access)**: users, template roles, custom roles, groups, page assignments.
- **System administration**: system configuration, constants / config values, notification templates, notification transactions, audit trail.
- **Notifications**: custom notifications (admin), My notifications page, header bell, background dispatcher.

Everything else from the main product (jobs, interviews, employment, credentials, reports) has been removed.

## Layout

| Folder | What it is |
|----|----|
| `Bidding_Models` | EF Core entities and DTOs (shared database schema) |
| `Bidding_Common` | Cross-cutting helpers (crypto, enums, constants) |
| `Bidding_DLL` | `CHDBContext`, data access classes, repositories, Identity Access seed data |
| `Bidding_Core` | Business services (auth, registration, identity access, admin system, notifications) |
| `Bidding_API` | ASP.NET Core 3.1 Web API (controllers, JWT, middleware, background jobs) |
| `Bidding_Core.Tests` | xUnit tests for rules and services |
| `TrainingModule` | Angular 19 client (standalone components, NgRx, `@angular/localize`) |
| `.cursor/rules` | Team rules the Cursor agent follows; read them, they are the coding standard |

## Run the API

Requirements: .NET Core SDK 3.1, SQL Server reachable with the shared database.

1. Edit `Bidding_API/appsettings.Development.json` and `Bidding_DLL/appsettings.json`:
   - `ConnectionStrings:SqlConnection`
   - `EmailSettings`, `SMSSettings`, `NafathJobSeeker` (values marked `<set-me>`)
2. Change the JWT signing keys in `Bidding_API/jwtConfigurations/JwtBearerTokenSecretKey.cs` and the matching copy in `Bidding_Core/Services/Implementations/AuthService.cs` before deploying anywhere.
3. `dotnet run --project Bidding_API`

## Run the client

```bash
cd TrainingModule
npm ci
npm start          # http://localhost:4200, talks to the API in src/environments
npm test           # Karma / Jasmine
```

## Database rules

- The schema and EF migrations are **owned by the main solution**. This repo has no `Migrations` folder and the API never calls `Database.Migrate()`.
- Need a new table or column? Add the entity here, then ask the platform team to add the migration in the main solution.
- The Identity Access seeder (app pages, template roles) only runs when `RunIdentityAccessSeeder` is `true` in appsettings. Leave it `false` on the shared database unless agreed.

## Conventions you must follow

The full list is in `.cursor/rules/` and `TrainingModule/docs/DESIGN-GUIDE.md`. The short version:

- **Forms**: validators from `@validators`, errors through `ValidationMessagePipe`, submit through `FormPipelineService`, HTTP through `AsyncExecutorService.execute()`.
- **Loading and toasts**: never add your own spinners; `AsyncExecutorService` drives the global `uiAsync` overlay.
- **Errors**: `httpErrorInterceptor` + `GlobalErrorHandler` + server `ExceptionMiddleware`; do not swallow errors in components.
- **i18n**: every UI string is `i18n="@@jobseeker.<area>.<key>"` or `$localize`; Arabic lives in `src/app/core/translations/<area>/messages.ar.json` and is registered in `merge-ar-translations.ts`.
- **Lists**: server-side paging, page size 5/10/15, `#` column, search on change, filter drawer; see `list-page-layout.mdc`.
- **New pages**: register them in `Bidding_DLL/Seeding/IdentityAccessAppPagesSeedData.cs` with the same URL as the route.
- **Rules on the UI are also enforced in the API.**
- **Every change ships with 2–3 tests.**

## Known local issue

`EmailSendRulesTests` hangs on Apple Silicon when the x64 .NET 3.1 runtime runs `AesGcm` under Rosetta. Run the other classes with `dotnet test --filter` or run on Windows/Linux.
