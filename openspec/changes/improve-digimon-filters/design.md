# Design: Improve Digimon Filters

## Technical Approach

Add supported filters incrementally to the existing Blazor Interactive Server list page. `DigimonList.razor` will keep owning list state, while a new `DigimonFilters` component renders level, attribute, and tri-state X-Antibody controls. Level/attribute options come from Digi-API metadata (`/level`, `/attribute`) and are cached through `DigiApiClient`. Selected values must use verified Digi-API names (`Child`, `Adult`, `Vaccine`, etc.), not localized aliases like `Rookie`.

## Architecture Decisions

| Decision | Choice | Alternatives considered | Rationale |
|---|---|---|---|
| Metadata source | Load level/attribute from Digi-API reference endpoints | Hardcoded values | Keeps UI aligned with API; still cache to avoid repeated calls. |
| Filter ownership | Page owns selected filter state; child component emits changes | Global state service | Current app uses local page state; global state would overbuild the MVP. |
| Metadata pagination | Follow returned `nextPage`/safe page walk rather than trusting `totalPages` alone | Stop on `totalPages` | Verified metadata reports odd pagination; `totalPages` is not reliable enough. |
| Type filter | Do not expose it | Client-side/detail-based type filtering | `/digimon` has no verified `type` parameter; detail-based filtering would create expensive N+1 calls. |
| Testing seam | Add focused client/component test coverage before feature code | Only Playwright smoke tests | Strict TDD is enabled; real-API E2E is too flaky for query/metadata behavior. |

## Data Flow

```text
DigimonList.razor
  ├─ OnInitializedAsync -> GetFilterOptionsAsync() -> /level + /attribute -> cache
  ├─ DigimonSearchBox -> _searchTerm
  ├─ DigimonFilters -> _selectedLevel/_selectedAttribute/_selectedXAntibody
  └─ LoadCurrentModeAsync -> GetDigimonPageAsync/GetAllDigimonAsync -> /digimon query
```

Filter changes normalize empty selections to `null`, reset `_currentPage` to `0`, preserve display mode, and reload the current mode. Option-loading errors are shown near the toolbar without raw exception details; list-loading errors remain separate.

## File Changes

| File | Action | Description |
|---|---:|---|
| `src/DigiDex.Web/Components/Digimon/DigimonFilters.razor` | Create | Reusable filter UI with selects and tri-state X-Antibody. |
| `src/DigiDex.Web/Components/Pages/DigimonList.razor` | Modify | Hold filter option/selection state, pass criteria to list/full-list loads, reset page on changes. |
| `src/DigiDex.Web/Services/IDigiApiClient.cs` | Modify | Add metadata option method(s). |
| `src/DigiDex.Web/Services/DigiApiClient.cs` | Modify | Fetch, map, cache metadata; use existing safe query construction for filters. |
| `src/DigiDex.Web/Models/Api/ReferenceDataDto.cs` | Create | DTOs for `{ name, description, fields[], pageable }` and field items. |
| `src/DigiDex.Web/Models/ViewModels/DigimonFilterOptionViewModel.cs` | Create | Immutable option model: id/name/href. |
| `src/DigiDex.Web/wwwroot/app.css` | Modify | Responsive toolbar/filter-control layout. |
| `tests/DigiDex.Web.Tests/` or existing test project | Create/Modify | Focused TDD tests for client mapping/query and component behavior. |
| `docs/digi-api-integration.md`, `docs/ui-design.md`, `CHANGELOG.md`, `Directory.Build.props` | Modify | Required project documentation, changelog, and version traceability during apply. |

Counts: 4 new, 7 modified, 0 deleted.

## Interfaces / Contracts

```csharp
Task<IReadOnlyList<DigimonFilterOptionViewModel>> GetLevelOptionsAsync(CancellationToken cancellationToken = default);
Task<IReadOnlyList<DigimonFilterOptionViewModel>> GetAttributeOptionsAsync(CancellationToken cancellationToken = default);

public sealed record class DigimonFilterOptionViewModel(int Id, string Name, string? Href);
```

Existing `GetDigimonPageAsync` and `GetAllDigimonAsync` contracts already accept `level`, `attribute`, and `bool? xAntibody`; implementation should pass all selected criteria consistently.

## Testing Strategy

| Layer | What to Test | Approach |
|---|---|---|
| Unit | Reference DTO mapping, metadata pagination by `nextPage`, cache behavior, query parameters. | Add fake `HttpMessageHandler` tests before service changes. |
| Component | Filter controls render options, emit changes, reset page, preserve selected criteria. | Keep coverage in xUnit/Playwright for now; do not add bUnit in this change. |
| E2E | User can select a verified filter such as `Child` or `Vaccine` and still search/open details. | Extend Playwright smoke coverage minimally; avoid asserting brittle API totals. |

## Migration / Rollout

No data migration required. Roll out as optional filters: unselected controls preserve current behavior. If metadata loading fails, listing remains usable with a visible toolbar error.

## Open Questions

- [x] Use existing xUnit/Playwright coverage; do not add bUnit for this change.
- [x] If option loading fails, disable filter selects and show a clear toolbar error; do not allow manual fallback values in the MVP.
