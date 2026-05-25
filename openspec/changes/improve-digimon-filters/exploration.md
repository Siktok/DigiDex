## Exploration: improve-digimon-filters

### Current State
DigiDex currently exposes the Digimon listing at `/` and `/digimon` through `src/DigiDex.Web/Components/Pages/DigimonList.razor`. The page has one active discovery control: `DigimonSearchBox`, which debounces name search and calls `SearchAsync`. The page also has a display-mode toggle between paginated cards and a full list.

The service boundary is already prepared for some filters: `IDigiApiClient.GetDigimonPageAsync` and `GetAllDigimonAsync` accept `name`, `level`, `attribute`, and `xAntibody`. `DigiApiClient` safely appends those optional query-string parameters to `GET /digimon`. However, `DigimonList.razor` only passes `_searchTerm`; there is no UI state or component for level, attribute, or X-Antibody filters.

The checked OpenAPI context documents `GET /digimon` parameters for `name`, `exact`, `attribute`, `xAntibody`, `level`, `page`, and `pageSize`. It does not document a `type` query parameter for the Digimon list, even though `/type` resource endpoints exist. Existing docs also state that level, attribute, and X-Antibody parameters are prepared but not surfaced in UI. No main OpenSpec specs currently exist under `openspec/specs/`.

Testing is currently E2E-focused with Playwright in `tests/DigiDex.Web.E2ETests`; there is no unit or integration test project for query generation, DTO mapping, or filter behavior. OpenSpec config has `strict_tdd: true`, so a proposal/design should account for tests before implementation.

### Affected Areas
- `src/DigiDex.Web/Components/Pages/DigimonList.razor` — owns listing state, loading/error/empty/success handling, pagination, full-list mode, and currently passes only name search into the API client.
- `src/DigiDex.Web/Components/Digimon/DigimonSearchBox.razor` — current search control; filter UX should align with its debounce/clear behavior without overloading this component.
- `src/DigiDex.Web/Services/IDigiApiClient.cs` — already includes filter parameters for list and full-list loading; may need metadata methods if select options are loaded from Digi-API.
- `src/DigiDex.Web/Services/DigiApiClient.cs` — already builds `level`, `attribute`, and `xAntibody` query parameters; may need cached reference-data calls and testable query behavior.
- `src/DigiDex.Web/Models/Api/` — may need DTOs for `/level` and `/attribute` list responses if option selects are populated dynamically.
- `src/DigiDex.Web/Models/ViewModels/` — may need filter option and selected-filter view models if the UI moves beyond primitive fields.
- `src/DigiDex.Web/wwwroot/app.css` — toolbar layout already supports controls; filters need responsive styling without breaking mobile layout.
- `docs/digi-api-integration.md` and `docs/ui-design.md` — should be updated when implementation documents active filters and UI conventions.
- `CHANGELOG.md` and `Directory.Build.props` — implementation phase will need a human-readable changelog entry and semantic version bump for a significant user-facing feature.
- `tests/` — strict TDD likely requires adding focused tests for filter query construction and UI behavior before code changes.

### Approaches
1. **Expose existing filters with simple manual controls** — Add a `DigimonFilters` component with text inputs/select-like primitives for `level`, `attribute`, and a tri-state X-Antibody control, then pass selected values into `GetDigimonPageAsync` and `GetAllDigimonAsync`.
   - Pros: Smallest implementation; uses the existing service contract; no new API endpoints or DTOs required; low risk to current listing/search behavior.
   - Cons: Poorer UX if users must know valid level/attribute names; more invalid or empty-result combinations; less discoverable than populated options.
   - Effort: Low

2. **Server-backed filter options for level/attribute plus X-Antibody** — Add cached reference-data methods for supported option lists, populate a reusable `DigimonFilters` component with valid levels and attributes, and keep X-Antibody as a tri-state control. Apply filters consistently in card and full-list modes, resetting pagination on changes.
   - Pros: Best MVP balance: discoverable UX, reuses Digi-API as source of truth, aligns with existing `IMemoryCache` guidance for repeated metadata, and avoids unsupported `type` filtering on `/digimon`.
   - Cons: Requires verifying `/level` and `/attribute` response shapes before coding; adds DTOs/mapping and more tests; must handle option-loading errors separately from list-loading errors or with a clear combined state.
   - Effort: Medium

3. **Client-side advanced filtering including type** — Load the full Digimon list and/or detail data, then filter in memory for criteria not supported by `GET /digimon`, such as type.
   - Pros: Could support richer filters than the list endpoint currently documents.
   - Cons: High network cost, likely N+1 detail calls for type, worse Interactive Server latency, more cache complexity, and too heavy for the learning-focused MVP. This would optimize for breadth before foundations.
   - Effort: High

4. **URL-query driven filters as the primary design** — Represent search, page, mode, level, attribute, and X-Antibody in the browser URL so filtered views are shareable and refresh-safe.
   - Pros: Better navigation semantics and shareability; useful once filters become a core feature.
   - Cons: More complexity in Blazor component lifecycle, URL parsing, validation, and synchronization; can be layered on after basic filters work.
   - Effort: Medium/High

### Recommendation
Use Approach 2 as the proposal baseline: implement supported, server-backed filters for level, attribute, and X-Antibody using a dedicated `DigimonFilters` component and cached option loading. This respects the current architecture, uses the filters already present in the service boundary, and avoids inventing unsupported `type` filtering.

Keep the first implementation intentionally incremental: preserve the existing search box, reset pagination when filters change, apply the same criteria to paginated cards and full-list mode, and show clear loading/empty/error states. Treat type filtering and URL-query persistence as follow-up work unless the user explicitly prioritizes them.

Because `strict_tdd` is enabled and the current test suite lacks unit/integration coverage, the next phases should plan a small test seam before implementation: at minimum tests for query construction/filter propagation and one UI/E2E scenario for applying a filter with search.

### Risks
- Digi-API reference endpoint response shapes for `/level` and `/attribute` must be verified before adding DTOs; do not infer them from detail DTOs alone.
- Combining filters with full-list mode can multiply API calls because `GetAllDigimonAsync` walks pages; bad filter combinations should stop promptly via pagination metadata.
- Option-loading failure can block the whole toolbar unless designed with graceful degradation.
- Adding a `type` filter now would be misleading unless the list endpoint is verified to support it or the implementation accepts expensive client-side/detail-based filtering.
- Strict TDD may require creating a focused test project before feature code, because current Playwright tests use the real Digi-API and are not ideal for query-construction coverage.

### Ready for Proposal
Yes. The topic is ready for `sdd-propose` with scope limited to level, attribute, and X-Antibody filters, plus explicit non-goals for type filtering, persistence, database changes, authentication, and heavy state management. The orchestrator should tell the user that the recommended MVP improvement is server-backed, cached filter options with a reusable Blazor filter component and tests planned first.
