# Tasks: Improve Digimon Filters

## Review Workload Forecast

| Field | Value |
|-------|-------|
| Estimated changed lines | 520-760 |
| 400-line budget risk | High |
| Chained PRs recommended | Yes |
| Suggested split | PR 1 client metadata/tests -> PR 2 UI filters -> PR 3 E2E/docs/version |
| Delivery strategy | ask-on-risk |
| Chain strategy | size-exception |

Decision needed before apply: No
Chained PRs recommended: Yes
Chain strategy: size-exception
400-line budget risk: High

### Suggested Work Units

| Unit | Goal | Likely PR | Notes |
|------|------|-----------|-------|
| 1 | Add metadata contracts, DTO mapping, cache, query tests | PR 1 | Base main; no UI dependency. |
| 2 | Add Blazor filter toolbar and list wiring | PR 2 | Depends on PR 1; includes CSS. |
| 3 | Add Playwright coverage, docs, changelog, version | PR 3 | Depends on PR 2; final verification. |

## Phase 1: RED - Service Contracts and Metadata

- [x] 1.1 Add failing xUnit coverage for `DigiApiClient` level/attribute metadata mapping from `{ name, description, fields[], pageable }`.
- [x] 1.2 Add failing xUnit coverage for metadata pagination following `nextPage` safely despite odd `totalPages`.
- [x] 1.3 Add failing xUnit coverage that `GetDigimonPageAsync` sends `level`, `attribute`, and `xAntibody` query values unchanged.
- [x] 1.4 Create `tests/DigiDex.Web.Tests/` only if needed for these non-E2E tests; reference `src/DigiDex.Web`.

## Phase 2: GREEN - API Client Foundation

- [x] 2.1 Create `Models/Api/ReferenceDataDto.cs` and `Models/ViewModels/DigimonFilterOptionViewModel.cs`.
- [x] 2.2 Extend `IDigiApiClient.cs` with `GetLevelOptionsAsync` and `GetAttributeOptionsAsync`.
- [x] 2.3 Implement metadata loading, mapping, conservative `IMemoryCache`, safe `nextPage` walk, and user-safe errors in `DigiApiClient.cs`.
- [x] 2.4 Keep filter values as verified Digi-API names such as `Child`, `Adult`, and `Vaccine`; do not introduce aliases.

## Phase 3: RED/GREEN - UI Filter Behavior

- [x] 3.1 Add failing Playwright/xUnit checks for visible level, attribute, X-Antibody controls and absence of type filter.
- [x] 3.2 Create `Components/Digimon/DigimonFilters.razor` with disabled selects plus toolbar error when options fail.
- [x] 3.3 Update `DigimonList.razor` to load options, own selected values, reset `_currentPage`, and pass filters to paginated/full-list loads.
- [x] 3.4 Update `wwwroot/app.css` for responsive toolbar controls without disrupting existing search/view layout.

## Phase 4: RED/GREEN - Scenarios and Result States

- [x] 4.1 Add failing tests for combined search+filters, page reset, and preserving criteria when switching display modes.
- [x] 4.2 Implement any missing list-state handling so filtered empty results remain distinct from list-loading failures.
- [x] 4.3 Extend Playwright smoke coverage minimally for selecting `Child` or `Vaccine` and opening a detail page.

## Phase 5: REFACTOR, Docs, and Verification

- [x] 5.1 Refactor duplicated normalization/error text while keeping page-local state and no global store.
- [x] 5.2 Update `docs/digi-api-integration.md`, `docs/ui-design.md`, `CHANGELOG.md`, and bump `Directory.Build.props` minor version.
- [x] 5.3 Run `dotnet test` and `dotnet build`; fix regressions before marking tasks complete.
