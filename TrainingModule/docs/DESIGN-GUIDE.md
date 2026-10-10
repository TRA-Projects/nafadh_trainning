# Jobseeker module — design guide

This document describes the **patterns, features, and conventions** used in the Angular job seeker client (`JobseekerModule`). Use it for onboarding and to keep new work aligned with the existing design.

---

## 1. Technology stack

| Area | Choice | Notes |
|------|--------|--------|
| Framework | Angular (standalone components) | Prefer `standalone: true`, `imports: [...]` |
| State | NgRx (`@ngrx/store`) | Feature slices per domain (e.g. `locale`, `jobseeker-profile`, `ui-async`) |
| HTTP | `HttpClient` + interceptors | Single pipeline: mock → base URL → error handling |
| i18n | `@angular/localize` (`$localize`, template `i18n`) | Runtime Arabic via `loadTranslations` + JSON catalog |
| Forms | Reactive forms when using the full pipeline | Validators + optional async validators |

---

## 2. Internationalization (i18n)

**Goals:** One codebase; English as source; Arabic (and future locales) loaded at runtime for Arabic users.

**Mechanisms:**

1. **Template strings** — `i18n="@@jobseeker.feature.key"` with a stable custom ID (`@@...`).
2. **TypeScript strings** — `$localize`:@@jobseeker.feature.key:English default``.
3. **Extraction** — `ng extract-i18n --format json --output-path src/locale --out-file messages.extract.json` to refresh keys.
4. **Arabic file** — `src/locale/messages.ar.json`: same message IDs as extract (without `@@`), under `translations`.
5. **Bootstrap** — `main.ts` loads `messages.ar.json` and calls `loadTranslations()` when persisted locale is `ar`. Switching language persists locale and **reloads** the app so `$localize` picks up the catalog.
6. **Locale ID** — `LOCALE_ID` is provided from persisted locale (for pipes like `DatePipe`).

**Follow:** Always use stable `@@` IDs for user-visible strings. Until strings are migrated to `$localize` / `i18n`, use **`LocaleService.text(en, ar)`** (English first, Arabic second) instead of inline `locale() === 'ar' ? … : …` ternaries in templates and TypeScript.

---

## 3. API surface and data access

**Single path catalog** — `src/app/core/api/api-paths.ts` (`ApiPaths`).

- Real backend and **mock** interceptor use the **same URLs**, so switching environments does not change call sites.

**Abstraction over HTTP** — `JOBSEEKER_PROFILE_ACTIONS` injection token (`jobseeker-profile-actions.token.ts`) with interface `JobSeekerProfileActions`.

- Default implementation: `JobseekerProfileApiService` (`core/api/services/jobseeker-profile-api.service.ts`).
- **Why:** Swap mocks, tests, or “view as user” without touching components.

**Request body naming** — Align with backend conventions (e.g. PascalCase `ProfileId`, `ServicesOffered: [{ Id }]` where the API expects objects, not raw number arrays).

---

## 4. HTTP interceptors (order matters)

Typical order (see `app.config.ts`):

1. **Mock backend** — When `environment.useMockBackend` is true, serves JSON from `src/assets/mocks/` for matching routes; mutations can return `{ ok: true }`.
2. **API base URL** — Prefixes relative paths with the configured API origin.
3. **HTTP error** — Normalizes errors / logging for failed requests.

**Follow:** Add new routes to `ApiPaths` and, for mock mode, to `mock-backend.interceptor.ts` + assets.

---

## 5. Async UI and “execute” pattern

**`AsyncExecutorService`** (`core/ui/async-executor.service.ts`):

- Wraps async work (usually HTTP).
- Dispatches to NgRx **`ui-async`** slice: loading overlay, last success/error snapshot.
- Options: `showProgress`, `successMessage`, `onSuccess`, `onError`, `finalize`, `recordSuccess`.

**Analog:** Freelancer’s `errorSuccessHandler().execute(...)`.

### 5.1 Global loading (single overlay)

**One place for blocking “please wait” UI:** `MainLayoutComponent` binds to `uiAsync` (`selectUiAsyncInProgress`, `progressMessage`) and renders a full-screen dimmed overlay with spinner while work is in flight.

**Rules:**

1. **Initial page / tab loads** that fetch data over HTTP should run inside `AsyncExecutorService.execute(...)` with `recordSuccess: false` so the **global overlay** appears, not an inline “Loading…” paragraph. Optionally set `progressMessage` to a short, task-specific line (shown under the spinner).
2. **Mutations** (save, delete, upload) also go through `execute()`; keep `recordSuccess: true` (default) when a success toast/snackbar is desired, or set `successMessage`.
3. **Do not** add per-component `loading` signals whose only job is to toggle a local “Loading…” label while `HttpClient` runs—use the global overlay instead. You may still keep a `loaded` (or store-derived) flag if the template must avoid rendering half-formed content until the first fetch completes.
4. **`FormPipelineService`** should end with `AsyncExecutorService.execute` for submit; that path is already consistent with this rule.

**Follow:** Use `execute()` for **all** user-visible async work that should block the shell (loads and saves). Avoid duplicating global loading logic in components.

---

## 6. Form validation

### 6.1 Custom validators

**Location:** `src/app/core/validators/`

- **String / number / file / select / checkbox / array / cross-field / date** — split into focused files.
- **Stable error keys** — `ValidationErrorKeys` + `validation-error-keys.ts` for `control.errors` and templates.
- **Global import:** `import { ... } from '@validators'` (path alias in `tsconfig.json`).

**Built-in Angular validators** re-exported as `AngularValidators` from `@validators` to avoid name clashes with custom helpers.

### 6.2 Showing messages

- **`firstValidationMessage(errors)`** — maps `ValidationErrors` to a single `$localize` string (`validation-messages.i18n.ts`).
- **`ValidationMessagePipe`** — `{{ control | validationMessage }}` in templates (when invalid + touched).
- **Arabic** — `messages.ar.json` includes `validation.*` keys; keep in sync when adding new `$localize` branches.

### 6.3 Backend (async) validators

**Helpers:** `core/ui/async-backend.validators.ts`

- `backendAsyncValidator(...)` — HTTP `Observable` → `ValidationErrors | null`.
- `debouncedBackendAsyncValidator(...)` — debounce for “check while typing”.

**Follow:** Put **field-level** remote checks on the control’s `asyncValidators` array. Use `updateValueAndValidity` + `waitForAllAsyncValidators` (or the form pipeline below) so `PENDING` is finished before submit.

---

## 7. Form pipeline (validate → optional server check → execute)

**`FormPipelineService`** (`core/ui/form-pipeline.service.ts`):

1. `markAllAsTouched()` (optional).
2. `form.updateValueAndValidity({ emitEvent: true })` — runs sync + async validators.
3. `waitForAllAsyncValidators(form)` — waits until no control is `PENDING`.
4. If invalid → return `{ ok: false, reason: 'invalid' }` (**no** generic submit error toast).
5. Optional **`asyncValidate()`** — extra server rules; set errors on controls; return `false` to abort.
6. **`AsyncExecutorService.execute(work, options)`** — the actual mutation.

**Types:** `FormPipelineOptions`, `FormPipelineResult`, `isFormPipelineSuccess()` (`form-pipeline.types.ts`).

**Utilities:** `form-async.utils.ts` (`flattenControls`, `waitForAllAsyncValidators`).

**Import barrel:** `import { FormPipelineService, ... } from '@core/ui'`.

**Follow:** Use this for multi-step flows that mirror Freelancer: **validate + optional backend check + execute + success/error handling**.

---

## 8. Feature folder layout

**Example:** `features/profile-completion/`

- One **folder per standalone component** (e.g. `jobseeker-complete-profile-page/`, `jobseeker-general-info-panel/`).
- Each folder contains `*.component.ts` + `*.component.html` (and styles if needed).

**Follow:** Keep feature UI cohesive; shared primitives live under `core/` or future `shared/`.

---

## 9. NgRx usage

- **Feature state** — `provideState(...)` in `app.config.ts` for each feature reducer.
- **Examples:** `locale`, `jobseeker-profile` (cached `me`), `ui-async` (**global** loading overlay + last success/error snapshot for `AsyncExecutorService`).

**Follow:** Put cross-cutting UI state (blocking progress, last toast) in `ui-async`; drive it via `AsyncExecutorService` (see §5.1), not parallel “isLoading” flags that only toggle local paragraphs.

---

## 10. Errors

- **`GlobalErrorHandler`** — central Angular error handling.
- **HTTP errors** — `http-error.interceptor.ts` (and executor’s `ui-async` error path for failed operations).

**Follow:** Validation failures are usually **inline on the form**; unexpected failures go through **interceptor / global handler / executor**.

---

## 11. Path aliases (`tsconfig.json`)

| Alias | Target |
|--------|--------|
| `@validators` | `src/app/core/validators/index.ts` |
| `@core/ui` | `src/app/core/ui/index.ts` |

**Follow:** Prefer these for shared validators and UI pipeline imports to keep paths short and consistent.

---

## 12. Add job (operator & admin only)

- **Guard** — `operatorOrAdminGuard` (`core/auth/auth.guard.ts`): only `VendorType` **0** (admin) or **2** / **4** (operator); everyone else is redirected to `AuthService.homePath()`.
- **Routes** — `/admin/jobs/post` and `/operator/jobs/post` share the same lazy-loaded `PostJobPageComponent` (`features/post-job/post-job-page/`).
- **API** — `JOB_POST_ACTIONS` token + `JobPostApiService` implementation; paths in `ApiPaths` (`jobsByProfile`, `jobsCategories`, `jobsSkills`). Draft body uses the same PascalCase DTO fields as legacy posting (`ProjectTitle`, `BudgetTimeline`, etc.); empty `Questions` / `Attachments` / `Phases` until extended.
- **UI pattern** — Reactive form, shared validators (`dateRangeOrder`, `arrayMinLength`, etc.), `ValidationMessagePipe`, submit via `FormPipelineService` → `AsyncExecutorService` with `$localize` success message; template IDs `@@jobseeker.postJob.*`.
- **Mock** — `assets/mocks/jobs-categories.json`, `jobs-skills.json`; `POST /api/v2/jobs/:profileId` returns `{ Data: { Id: … } }`. `GET /api/v2/auth/me` is persona-aware (uses `MOCK_USER_ID_STORAGE_KEY`) so operator/admin get the correct `ProfileId` when saving.

---

## 13. Checklist for new work

1. **Strings** — `$localize` or `i18n` with stable `@@` IDs; update `messages.ar.json` when adding keys.
2. **API** — Add to `ApiPaths`; extend `JobSeekerProfileActions` (or new token) if it’s a new domain.
3. **HTTP** — Implement through token + `HttpClient`; keep mock in sync if using mock mode.
4. **Loading UX** — **Required:** use `AsyncExecutorService` for initial loads and mutations so **`MainLayoutComponent`** shows the shared overlay (§5.1). Use `recordSuccess: false` for silent fetches.
5. **Forms** — **Required for new/refactored forms:** reuse validators from `@validators`; show errors with `ValidationMessagePipe` or `firstValidationMessage` (see §6). Do not default to bare `Validators.required` + manual checks in `save()` without team agreement.
6. **Submit** — **Required for new/refactored flows:** `FormPipelineService` + `AsyncExecutorService` (see §7). Reference: **Post job** page.
7. **Folders** — One folder per component inside the feature.
8. **Interceptors** — Register in `app.config.ts` in the correct order.

**Cursor / automation:** `.cursor/rules/form-validation-pipeline.mdc` (`alwaysApply`) reiterates §5–7 for agents and reviews; `.cursor/rules/ui-async-global-loading.mdc` reiterates §5.1 (global overlay vs duplicate spinners).

**New to Angular?** See **`JUNIOR-ONBOARDING-ADD-FEATURE-GUIDE.md`** (same folder; open **`JUNIOR-ONBOARDING-ADD-FEATURE-GUIDE.html`** in Word and save as `.docx` — see **`HOW-TO-OPEN-AS-DOCX.txt`**).

---

## 14. Relationship to the legacy Freelancer module

The older **Freelancer** stack (Alpine, `validations.js`, `errorSuccessHandler`) informed **patterns**, not code sharing:

| Freelancer | Angular job seeker |
|------------|--------------------|
| `validateWithError` + `{ val, rules, error }` | Reactive forms + `Validators` / custom `ValidatorFn` |
| `errorSuccessHandler().execute` | `AsyncExecutorService` + optional `FormPipelineService` |
| `ERROR_CODES` + translator | `ValidationErrorKeys` + `$localize` + `validation.*` in JSON |

This document is the **source of truth** for the Angular module; the legacy app is reference only.

---

*Last updated to match the Jobseeker Angular module design (locale, API, validators, form pipeline, async UI, add job for operator/admin).*
