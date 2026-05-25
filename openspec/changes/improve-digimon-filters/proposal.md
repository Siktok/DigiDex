# Proposal: Improve Digimon Filters

## Intent

Let users narrow the Digimon list by supported level, attribute, and X-Antibody filters while keeping the MVP simple and avoiding unsupported type filtering.

## Scope

### In Scope
- Add level, attribute, and tri-state X-Antibody filter UI.
- Load valid level/attribute options with cache.
- Apply filters to paginated and full-list modes; reset page on changes.
- Preserve loading, empty, error, and success states.
- Add strict-TDD coverage before implementation.

### Out of Scope
- Type filtering, URL query persistence, favorites, database, auth, EF Core, MediatR, AutoMapper, Redux-style state, or broad rewrites.

## Capabilities

### New Capabilities
- `digimon-filters`: User-facing list filtering by level, attribute, and X-Antibody, including option loading, result states, and consistent display-mode behavior.

### Modified Capabilities
- None; `openspec/specs/` has no existing capabilities.

## Approach

Follow exploration Approach 2: add `DigimonFilters`, keep `DigimonSearchBox` for name search, reuse existing `IDigiApiClient` filter parameters, and add cached metadata methods only after verifying `/level` and `/attribute`. Keep Blazor Interactive Server structure and write tests first.

## Affected Areas

| Area | Impact | Description |
|------|--------|-------------|
| `src/DigiDex.Web/Components/Pages/DigimonList.razor` | Modified | Own selected filters, reset page, pass criteria to loads. |
| `src/DigiDex.Web/Components/Digimon/DigimonFilters.razor` | New | Filter toolbar. |
| `src/DigiDex.Web/Services/IDigiApiClient.cs` | Modified | Add metadata methods if dynamic options are used. |
| `src/DigiDex.Web/Services/DigiApiClient.cs` | Modified | Verify/map/cache options; preserve safe queries. |
| `src/DigiDex.Web/Models/Api/` | Modified | Add verified option DTOs if needed. |
| `src/DigiDex.Web/Models/ViewModels/` | Modified | Add option/filter view models if clearer. |
| `tests/` | Modified | Add tests first. |
| `docs/`, `CHANGELOG.md`, `Directory.Build.props` | Modified | Docs, changelog, version bump. |

## Risks

| Risk | Likelihood | Mitigation |
|------|------------|------------|
| Option response shapes differ. | Medium | Verify real responses before DTOs/specs. |
| Option-load failure blocks listing. | Medium | Graceful toolbar error or fallback. |
| Full-list filters increase calls. | Medium | Filter server-side; stop via pagination metadata. |
| Real API tests become flaky. | Medium | Prefer HTTP simulation/test seams. |

## Rollback Plan

Revert filter component, metadata methods, DTO/ViewModel additions, tests, docs, changelog, and version bump. Existing search/list modes remain because filters are optional.

## Dependencies

- Digi-API `GET /digimon` support for `level`, `attribute`, and `xAntibody`.
- Verified `/level` and `/attribute` response shapes.
- Existing Blazor Interactive Server app and xUnit/Playwright runner.

## Success Criteria

- [ ] Users filter by level, attribute, and X-Antibody.
- [ ] Filters combine with name search, pagination, and full-list mode.
- [ ] Filter changes reset pagination and show result states.
- [ ] Type filtering is not exposed.
- [ ] Tests are written first and `dotnet test` passes.
