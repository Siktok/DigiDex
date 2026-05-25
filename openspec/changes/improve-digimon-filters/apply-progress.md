# Apply Progress: Improve Digimon Filters

## Mode

Strict TDD, artifact store `openspec`, workload strategy `size-exception`.

## Completed Tasks

- [x] 1.1 Add xUnit coverage for level/attribute metadata mapping.
- [x] 1.2 Add xUnit coverage for metadata pagination following `nextPage`.
- [x] 1.3 Add xUnit coverage for unchanged `level`, `attribute`, and `xAntibody` query values.
- [x] 1.4 Create `tests/DigiDex.Web.Tests/` and reference `src/DigiDex.Web`.
- [x] 2.1 Create reference data DTOs and filter option ViewModel.
- [x] 2.2 Extend `IDigiApiClient` with filter metadata methods.
- [x] 2.3 Implement metadata loading, mapping, cache, safe `nextPage` walking, and safe errors.
- [x] 2.4 Preserve Digi-API names without aliases.
- [x] 3.1 Add Playwright checks for filter controls and absence of type filter.
- [x] 3.2 Create `DigimonFilters.razor` with disabled selects and toolbar error.
- [x] 3.3 Wire `DigimonList.razor` filter state, page reset, and filtered loads.
- [x] 3.4 Update responsive toolbar CSS.
- [x] 4.1 Add tests for combined search+filters, page reset, and display-mode criteria preservation.
- [x] 4.2 Preserve filtered empty state separately from load failures.
- [x] 4.3 Extend Playwright smoke coverage for selecting `Child` and opening detail flow coverage through existing/detail assertions.
- [x] 5.1 Refactor filter normalization and clear behavior while keeping page-local state.
- [x] 5.2 Update docs, changelog, and bump version to `0.5.0`.
- [x] 5.3 Run `dotnet test` and `dotnet build` successfully.

## TDD Cycle Evidence

| Task | Test File | Layer | Safety Net | RED | GREEN | TRIANGULATE | REFACTOR |
|------|-----------|-------|------------|-----|-------|-------------|----------|
| 1.1 | `tests/DigiDex.Web.Tests/DigiApiClientFilterTests.cs` | Unit | ✅ Baseline `dotnet test`: 1/1 E2E passing | ✅ Missing `GetLevelOptionsAsync` compile failure | ✅ Unit suite 6/6 | ✅ Direct and `content` envelope mapping cases | ✅ DTO mapping centralized |
| 1.2 | `tests/DigiDex.Web.Tests/DigiApiClientFilterTests.cs` | Unit | ✅ Baseline captured | ✅ Missing metadata method compile failure | ✅ Unit suite 6/6 | ✅ `totalPages` oddity plus `nextPage` stop | ✅ Defensive max page constant |
| 1.3 | `tests/DigiDex.Web.Tests/DigiApiClientFilterTests.cs` | Unit | ✅ Baseline captured | ✅ Query assertions written before related changes | ✅ Unit suite 6/6 | ✅ true and false `xAntibody` cases | ➖ Existing query path already clean |
| 1.4 | `tests/DigiDex.Web.Tests/DigiDex.Web.Tests.csproj` | Unit | N/A (new) | ✅ Project added before production metadata methods | ✅ Unit project runs 6/6 | ➖ Structural project setup | ✅ Added to `DigiDex.slnx` |
| 2.1-2.4 | `tests/DigiDex.Web.Tests/DigiApiClientFilterTests.cs` | Unit | ✅ Service tests green after each cycle | ✅ Tests referenced missing contracts first | ✅ Unit suite 6/6 | ✅ Mapping, envelope, pagination, cache, query cases | ✅ User-safe reference read helper |
| 3.1-3.4 | `tests/DigiDex.Web.E2ETests/DigimonSmokeTests.cs` | E2E | ✅ Full baseline passed before UI edits | ✅ New E2E controls failed: labels not found | ✅ New E2E 2/2 | ✅ Control visibility plus no type filter | ✅ Component extracted as `DigimonFilters` |
| 4.1-4.3 | `tests/DigiDex.Web.E2ETests/DigimonSmokeTests.cs` | E2E | ✅ Existing E2E safety net | ✅ Combined flow failed before UI wiring | ✅ Full E2E 3/3 | ✅ Search + `Child` + `false`, then full-list preservation | ✅ Clear filters callback avoids multiple reloads |
| 5.1-5.3 | Full suite | Unit/E2E | ✅ Previous targeted tests green | ✅ Verification expected docs/version/tasks incomplete | ✅ `dotnet test` 9/9 and `dotnet build` 0 warnings/errors | ✅ Unit and E2E suites both included via solution | ✅ Changelog/docs/version synchronized |

## Test Summary

- **Total tests written**: 8 test methods (6 unit, 2 new E2E) plus existing E2E retained.
- **Total tests passing**: 9/9 under `dotnet test`.
- **Layers used**: Unit (6), E2E (3 total, 2 new).
- **Approval tests**: None — no pure refactoring-only task without behavior change.
- **Pure functions created**: 2 helper-style mapping/normalization paths in service/component code.

## Deviations from Design

- `ReferenceDataDto` supports both the prompt-described direct metadata shape and the actually observed Digi-API `content` envelope. This is an intentional robustness fix based on live API verification during apply.

## Issues Found

- `dotnet test` initially did not discover the new unit project until `DigiDex.slnx` was updated.
- Live `/level` returned `{ content: { name, description, fields[] }, pageable }`, not only the direct shape described in preflight.

## Verification

- `dotnet test` ✅ 9/9 passing.
- `dotnet build` ✅ 0 warnings, 0 errors.
