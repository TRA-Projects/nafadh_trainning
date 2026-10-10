# System config, taxonomy catalogs & server paging

Team guide for **JobseekerModule** + **Bidding_API** jobseeker admin APIs.

Use this when adding dropdowns, autocomplete fields, or any browsable list/table.

---

## 1. Two catalog systems (do not mix them up)

| Kind | Examples | Where it lives | How the UI reads it |
|------|----------|----------------|---------------------|
| **System config values** | Currency, pay frequency, contract type, nationality, rejection reasons | Admin → **Constants** (`/admin/system-administration/constants`) | `AdminSystemApiService.getValues(code)` |
| **Taxonomy entities** | Professional skills, basic skills, tech stacks, skill categories | Admin → **Taxonomy** (`/admin/system-administration/taxonomy`) | `AdminSystemApiService.getSkills()`, `getBasicSkills()`, `getTechStacks()`, … |

### System config — use for

- Fixed **option sets** (enums): statuses, types, reasons, currencies, modes.
- Bilingual rows: `label_en` / `label_ar`.
- Optional **parent/child** links via `parentReasonId` (e.g. interview rejection main reason → sub-reason).
- Values referenced by **`id`** (string or number) stored on domain records.

### Taxonomy — use for

- **CRUD catalogs** with richer structure (categories, active flag, numeric ids).
- Skills / tech stacks attached to profiles, jobs, credentials, interviews.
- Do **not** duplicate these in a feature-specific `GET …/lookups` endpoint.

### Do not

- ❌ Hardcoded `countries[]`, `reasons[]`, or `*-lookups.json` assets for new work.
- ❌ Bundled “lookups” endpoints that return multiple catalogs in one payload (e.g. `/registration-lookups`, `/employment-procedures/currencies`).
- ❌ Parallel catalogs for the same data (registration-only nationality when `NAT` already exists in config).

---

## 2. Reusing an existing system config group

Before creating anything new:

1. Open **`SYSTEM_CONFIG_CODES`** in  
   `JobseekerModule/src/app/core/profile/system-config-codes.ts`.
2. Check Admin → **Constants** for an matching group.
3. Reuse the **same code string** everywhere (TS constant value = API path segment = admin group key).

### Read APIs (frontend)

All via **`AdminSystemApiService`** (`core/api/services/admin-system-api.service.ts`):

| Need | Method | HTTP |
|------|--------|------|
| Full list for a `<select>` | `getValues(code)` | `GET /api/jobseekers/admin/system-config/values/{code}` (no paging params → all active rows) |
| Autocomplete / search | `searchConfigValues(code, query)` | `GET …/values/{code}/search?q=…` |
| User typed a new value (“Add …”) | `ensureConfigValue(code, draft)` | `POST …/values/{code}/ensure` |
| Admin Constants table (paged) | `getConfigValuesPage(code, pageIndex, pageSize, parentReasonId?)` | Same GET with `pageIndex` + `pageSize` |

**Base path:** `/api/jobseekers/admin/…` (see `api-paths.ts`).

### Forms — preferred UI

```html
<app-config-lookup-autocomplete
  formControlName="institution"
  [configCode]="SYSTEM_CONFIG_CODES.EDUCATIONAL_INSTITUTION"
/>
```

- Pass **`[configCode]`** from `SYSTEM_CONFIG_CODES` — never hardcode `'EDUC_INST'` in the component.
- Linked main + sub catalogs: use `[subControl]` + pair registered in `system-config-sub-codes.util.ts`.

### Read-only display (one language at a time)

Show **one** label for the active locale:

```ts
readonly label = computed(() =>
  this.locale.locale() === 'ar' ? row.label_ar : row.label_en
);
```

Do not show EN and AR side by side on detail/list cells.

### Mapping config → feature DTO

If a feature expects a different shape (e.g. employment `ContractTypeOption`), map in a **small util** — do not add a new API:

```ts
// Example: core/profile/employment-config-lookups.util.ts
contractTypesFromConfig(rows: SystemConfigValueDto[]): ContractTypeOption[]
```

Then in the feature API service:

```ts
this.adminApi.getValues(SYSTEM_CONFIG_CODES.CONTRACT_TYPE).pipe(
  map(contractTypesFromConfig),
  tap(rows => this.store.dispatch(cacheContractTypes({ rows }))),
);
```

---

## 3. Adding a **new** system config group

Checklist — complete **all** steps:

| # | Layer | Action |
|---|--------|--------|
| 1 | **TS constant** | Add `SYSTEM_CONFIG_CODES.YOUR_NAME = 'SHORT_CODE'` (~4 chars per segment, e.g. `CONT_TYPE`, `OFFR_RJCT`). |
| 2 | **Mock seed (TS)** | Add rows under `initialConfigValuesByCode` in `core/mocks/mock-admin-system.state.ts` (`id`, `code`, `label_en`, `label_ar`, optional `parentReasonId`, `subReasonDropdown`). |
| 3 | **Mock seed (C#)** | Mirror in `AdminSystemMockStore.BuildInitialConfigValuesByCode()` (`Bidding_Core/Services/Implementations/AdminSystemMockStore.cs`). |
| 4 | **Admin UI** | Register the group in `constants-manage-page.component.ts` → `allLookupTypes`. |
| 5 | **Arabic** | Add admin label keys in `core/translations/admin-system/messages.ar.json`. |
| 6 | **Consumer** | Wire feature via `AdminSystemApiService` or `app-config-lookup-autocomplete` — **no new lookup route**. |
| 7 | **Sub-catalog** (if needed) | Register main↔sub pair in `system-config-sub-codes.util.ts`; seed sub rows with `parentReasonId`. |

### Row shape (`SystemConfigValueDto`)

```ts
{
  id: string | number;      // stored on domain records
  code?: string;            // short stable code (optional display)
  label_en: string;
  label_ar: string;
  IsActive?: boolean;
  subReasonDropdown?: boolean;   // main row: show sub-reason picker in interview conduct
  parentReasonId?: string | number | null;  // sub row: points to main reason id
}
```

### Anonymous / pre-login screens

Registration and other public forms still call **`GET system-config/values/{code}`** for specific codes (e.g. `NAT`, `PRIM_SPEC`).  
Do **not** add a separate registration lookup bundle.

---

## 4. Reusing taxonomy (skills / tech stacks)

```ts
// Credential / interview pickers
this.adminApi.getSkills().pipe(map(skillRefsFromTaxonomy));
this.adminApi.getBasicSkills().pipe(map(basicSkillRefsFromTaxonomy));
this.adminApi.getTechStacks().pipe(map(techStackRefsFromTaxonomy));
```

Shared mappers: `core/profile/admin-taxonomy-lookup.util.ts`.

Admin CRUD: **Taxonomy** page + `GET/POST/PUT` under `/api/jobseekers/admin/skills`, `basic-skills`, `tech-stacks`.

---

## 5. Server paging contract

### Response shape (mandatory for paged lists)

```ts
interface PagedListResponse<T> {
  rows: T[];
  total: number;      // full count before slice
  pageIndex: number;  // 0-based, may be clamped by server
  pageSize: number;
}
```

Wrapped in API envelope: `{ Code: 'Success', Data: { rows, total, pageIndex, pageSize } }`.

Shared types: `core/api/models/paged-list.models.ts`.

### Query parameters

| Param | Type | Notes |
|-------|------|--------|
| `pageIndex` | int, 0-based | Omit with `pageSize` when requesting **full list** |
| `pageSize` | int | **`5`, `10`, or `15`** for UI tables; omit or `0` = return all rows |
| `q` | string | Optional search (server filters) |
| `parentReasonId` | string | Config values: filter sub-reasons |
| `categoryId` | int | Skills / basic skills taxonomy filter |

**Frontend rule:** if `pageSize > 0` and `pageIndex >= 0`, send both; otherwise omit paging params (full list).

**Backend rule:** if paging params are missing or invalid, return **all rows** with `pageIndex: 0`, `pageSize: total` (see `AdminSystemMockStore.SlicePage`).

### Allowed page sizes

```ts
export const PAGE_SIZE_OPTIONS = [5, 10, 15] as const;
export const DEFAULT_PAGE_SIZE = 5;
```

Backend should clamp unknown sizes to **5** (same as mock store).

---

## 5.1 Shared paging helpers (use these — do not reimplement)

Paging logic lives in **two small helpers per stack**. Controllers and components should **not** branch on “is this paged?” — pass optional params through and let the helper decide.

### Backend (C#) — `AdminSystemMockStore`

**File:** `Bidding_Core/Services/Implementations/AdminSystemMockStore.cs`

| Method | Purpose |
|--------|---------|
| **`ShouldApplyPaging(pageIndex, pageSize)`** | Returns `true` only when both params are present, `pageSize > 0`, and `pageIndex >= 0`. |
| **`SlicePage<T>(rows, pageIndex, pageSize)`** | Applies paging when `ShouldApplyPaging` is true; otherwise returns **all rows**. |

**Rules inside `SlicePage`:**

- **`total`** = full filtered count **before** slicing (never `rows.Count` after slice).
- If paging **does not** apply → `{ rows: all, total, pageIndex: 0, pageSize: total }`.
- If paging applies → clamp `pageSize` to **5, 10, or 15** (default 5), clamp `pageIndex` to valid range, then `Skip` / `Take`.
- Always return **`pageSize`** on the response DTO (admin uses `AdminSystemPagedListDto<T>`).

**Usage pattern (repository / mock store):**

```csharp
// 1. Filter/search first
var filtered = FilterSomething(allRows, q);

// 2. Slice — pass nullable query params straight through
return SlicePage(filtered, pageIndex, pageSize);
```

**Controller pattern — no dual branches:**

```csharp
[HttpGet("records")]
public async Task<IActionResult> GetRecords(
    [FromQuery] int? pageIndex,
    [FromQuery] int? pageSize,
    [FromQuery] string q = "")
{
    // Always one code path — SlicePage handles full list vs paged
    var data = await _service.GetRecordsPageAsync(pageIndex, pageSize, q);
    return Ok(new { Code = "Success", Data = data });
}
```

**Do not** copy-paste another `SlicePage` in a new service unless you are matching this contract exactly. Older feature services (`CredentialManagementService`, `InterviewManagementService`) still have local copies — **new work should follow `AdminSystemMockStore`**. When touching those files, prefer extracting a shared `PagingHelper` in `Bidding_Core` with the same two methods.

---

### Frontend (TypeScript) — `paged-list.utils.ts`

**File:** `JobseekerModule/src/app/core/utils/paged-list.utils.ts`

| Function | Purpose |
|----------|---------|
| **`normalizePageSize(raw)`** | Clamps to **5, 10, or 15** (default 5). |
| **`normalizePageIndex(pageIndex, total, pageSize)`** | Clamps index to `0 … totalPages - 1`. |
| **`slicePagedList(rows, pageIndex?, pageSize?)`** | **Main helper** — mirrors backend `SlicePage`. |

**Rules inside `slicePagedList`:**

- If `pageSize` is **null, undefined, or ≤ 0** → return all rows (`pageIndex: 0`, `pageSize: total`).
- Otherwise normalize size/index, slice, return `{ rows, total, pageIndex, pageSize }`.

**Usage (mock interceptor / in-memory mock state):**

```ts
import { slicePagedList } from '../utils/paged-list.utils';

// After filter/search:
return slicePagedList(filteredRows, pageIndex, pageSize);
```

**HTTP query params** — only send paging when actually paging (`AdminSystemApiService.pageParams`):

```ts
// pageSize > 0 and pageIndex >= 0 → set pageIndex & pageSize on HttpParams
// otherwise omit both → backend/mock returns full list
```

**Footer label:** use `pageFooterLabel(total, pageIndex, pageSize)` from the same file (wired to `$localize`).

---

## 6. Frontend paging — tables & lists

Every **user-browsable table** must be paginated (see also `.cursor/rules/table-pagination.mdc`).

### UI requirements

1. Default page size **5**; selector **5 / 10 / 15**.
2. First column **`#`**: `safePageIndex * pageSize + i + 1`.
3. Footer: **Showing X–Y of Z**, Prev / Next (disabled at bounds).
4. **Reset `pageIndex` to 0** when search, filters, sort, or page size changes.
5. **Server-driven:** pass `pageIndex` / `pageSize` on the HTTP call; **do not** `.slice()` client-side when an API exists.

### Component signals (skeleton)

```ts
const PAGE_SIZE_OPTIONS = [5, 10, 15] as const;
readonly pageSize = signal<5 | 10 | 15>(5);
readonly pageIndex = signal(0);

readonly totalCount = computed(() => this.pageFromApi()?.total ?? 0);
readonly totalPages = computed(() => Math.max(1, Math.ceil(this.totalCount() / this.pageSize())));
readonly safePageIndex = computed(() =>
  Math.min(this.pageIndex(), Math.max(0, this.totalPages() - 1))
);
readonly canPrev = computed(() => this.safePageIndex() > 0);
readonly canNext = computed(() => this.safePageIndex() < this.totalPages() - 1);

setPageSize(s: 5 | 10 | 15) {
  this.pageSize.set(s);
  this.pageIndex.set(0);
  this.refetch();
}
nextPage() { if (this.canNext()) { this.pageIndex.update(p => p + 1); this.refetch(); } }
prevPage() { if (this.canPrev()) { this.pageIndex.update(p => p - 1); this.refetch(); } }
```

Template binds **`rows` from API** (`page.rows`), not a client slice.

### NgRx pattern

1. HTTP service method: `(params) => GET …?pageIndex=&pageSize=&q=`.
2. On success: `dispatch(cacheXxxPage({ rows, total, pageIndex, pageSize }))`.
3. Component reads from selector; refetch on page/filter/size change and after mutations that affect the list.

Reference: `AdminSystemApiService.getConfigValuesPage`, `InterviewManagementApiService.getInterviewRecordsPage`, `CredentialManagementApiService.getEvaluationsPage`.

### Dropdowns vs tables

| UI | Paging |
|----|--------|
| **Table** (Constants, interview records, credentials, …) | Always **server-paged** (`pageSize` 5/10/15). |
| **Dropdown / autocomplete** (select, config lookup) | Usually **full list**: `getValues(code)` or `getSkills()` with no paging params. |
| **Large taxonomy picker** | May use search endpoint (`searchConfigValues`) instead of loading all rows. |

---

## 7. Backend paging — implementers

See **§5.1** for `ShouldApplyPaging` + `SlicePage` — that is the canonical backend implementation today.

### Controller

```csharp
[HttpGet("records")]
public async Task<IActionResult> GetRecords(
    [FromQuery] int? pageIndex,
    [FromQuery] int? pageSize,
    [FromQuery] string q = "")
{
    var data = await _service.GetRecordsPageAsync(pageIndex, pageSize, q);
    return Ok(new { Code = "Success", Data = data });
}
```

### Service / mock store

1. Filter in memory (or DB) **first**.
2. Call **`SlicePage(filtered, pageIndex, pageSize)`** — do not hand-roll `Skip`/`Take` in the controller.
3. Return `PagedListResponseDto<T>` / `AdminSystemPagedListDto<T>` with **`total`**, **`pageIndex`**, and **`pageSize`**.

### Mock interceptor (Angular)

- Parse `pageIndex`, `pageSize`, `q` from query string.
- Call **`slicePagedList(filteredRows, pageIndex, pageSize)`** — same rules as C# `SlicePage`.
- Return `{ Data: { rows, total, pageIndex, pageSize } }`.
- Keep mock data **cross-linked** with canonical personas (see `.cursor/rules/single-mock-persona.mdc`).

---

## 8. Environment / hybrid dev

| Flag | Effect |
|------|--------|
| `useMockBackend: false` | Most jobseeker admin calls hit **Bidding_API** |
| `useAdminSystemMock: true` | Admin config/taxonomy served from in-memory TS mock (no HTTP) |
| `useMockBackend: true` | Mock interceptor handles admin routes |

**Lookups do not need feature-specific “real vs mock” flags.**  
Config and taxonomy always go through **`AdminSystemApiService`** → either mock interceptor or `AdminSystemController`.

---

## 9. Quick decision tree

```
Need options for a dropdown?
├─ Fixed enum-like set (currency, reason, mode, type)?
│  └─ System config → SYSTEM_CONFIG_CODES + getValues / config-lookup-autocomplete
├─ Skill, tech stack, or category?
│  └─ Taxonomy → getSkills / getTechStacks / getBasicSkills
└─ Feature-specific entity list (candidates, records, jobs)?
   └─ Domain API with server paging — NOT a lookup catalog
```

```
Building a table?
├─ Pass pageIndex + pageSize (5|10|15) to API
├─ Store { rows, total, pageIndex, pageSize } in NgRx
├─ Number rows # with page-aware index
└─ Refetch on filter/size/page change — never client-slice server lists
```

---

## 10. Reference files

| Topic | Path |
|-------|------|
| Config codes | `JobseekerModule/src/app/core/profile/system-config-codes.ts` |
| Admin API service | `JobseekerModule/src/app/core/api/services/admin-system-api.service.ts` |
| Config autocomplete | `JobseekerModule/src/app/core/ui/config-lookup-autocomplete/` |
| Config → feature mapping example | `JobseekerModule/src/app/core/profile/employment-config-lookups.util.ts` |
| Taxonomy mapping | `JobseekerModule/src/app/core/profile/admin-taxonomy-lookup.util.ts` |
| Paged types | `JobseekerModule/src/app/core/api/models/paged-list.models.ts` |
| **Frontend paging helpers** | `JobseekerModule/src/app/core/utils/paged-list.utils.ts` → `normalizePageSize`, `normalizePageIndex`, `slicePagedList` |
| C# admin API | `Bidding_API/Controllers/Jobseekers/AdminSystemController.cs` |
| **Backend paging helpers** | `Bidding_Core/Services/Implementations/AdminSystemMockStore.cs` → `ShouldApplyPaging`, `SlicePage` |
| Cursor rules (AI) | `.cursor/rules/system-config-lookups-only.mdc`, `table-pagination.mdc`, `config-lookup-autocomplete.mdc` |

---

## 11. PR checklist (copy into review)

- [ ] No new bundled lookup endpoint or hardcoded option array
- [ ] New config group added to `SYSTEM_CONFIG_CODES` + both mock seeds + Constants UI + AR strings
- [ ] UI reads via `AdminSystemApiService` or `app-config-lookup-autocomplete`
- [ ] Read-only views show one locale per bilingual field
- [ ] Tables use server paging (5/10/15), `#` column, Showing X–Y of Z
- [ ] Backend uses `ShouldApplyPaging` + `SlicePage` (or `slicePagedList` in mock interceptor) — no controller `if (paged)` branches
- [ ] Mock interceptor honors same query params and response shape as C#
- [ ] Mutations that change list content trigger refetch (or store update)
