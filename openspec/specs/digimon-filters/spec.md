# Digimon Filters Specification

## Purpose

Define user-facing filtering for the Digimon list by supported Digi-API criteria: level, attribute, and X-Antibody. This capability SHALL keep filtering discoverable, optional, and consistent across list display modes.

## Requirements

### Requirement: Filter controls

The system MUST expose filters for level, attribute, and X-Antibody on the Digimon list. It MUST NOT expose type filtering for this capability. Level and attribute choices SHOULD come from valid Digi-API reference data. X-Antibody MUST support all, yes, and no states.

#### Scenario: Supported filters are visible

- GIVEN the Digimon list page is displayed
- WHEN filter controls are rendered
- THEN level, attribute, and X-Antibody controls are available
- AND no type filter is available

#### Scenario: Optional filters preserve default results

- GIVEN no filter value is selected
- WHEN the list loads
- THEN the system MUST show the same unfiltered Digimon results as the existing list behavior

### Requirement: Filter application

The system MUST apply selected level, attribute, and X-Antibody filters together with the current name search. Filter changes MUST reset pagination to the first page and MUST apply to both paginated-card and full-list display modes.

#### Scenario: Combined criteria narrow results

- GIVEN a user has entered a name search and selected level, attribute, or X-Antibody filters
- WHEN the list reloads
- THEN results MUST match the combined selected criteria

#### Scenario: Filter change resets page

- GIVEN the user is viewing a page after the first page
- WHEN the user changes any filter value
- THEN the selected page MUST reset to the first page

#### Scenario: Display mode keeps criteria

- GIVEN filters are selected in paginated-card mode
- WHEN the user switches to full-list mode
- THEN the same selected filters MUST remain applied

### Requirement: Filter option loading states

The system MUST handle loading, success, empty, and error states for filter option data. An option-loading failure MUST be visible to the user and MUST NOT expose raw exception details.

#### Scenario: Options load successfully

- GIVEN level and attribute options are available
- WHEN the page finishes loading filter metadata
- THEN the controls MUST present selectable level and attribute options

#### Scenario: Options fail to load

- GIVEN filter option loading fails
- WHEN the toolbar is displayed
- THEN a clear user-facing error MUST be shown
- AND raw technical exception details MUST NOT be shown

### Requirement: Filtered result states

The system MUST preserve loading, success, empty, and error states while applying filters. Empty filtered results MUST be distinguishable from load failures.

#### Scenario: Filtered results are empty

- GIVEN selected filters match no Digimon
- WHEN the list reloads successfully
- THEN an empty-state message MUST be displayed
- AND the state MUST NOT be treated as an error

#### Scenario: Filtered results fail to load

- GIVEN selected filters are applied
- WHEN loading Digimon results fails
- THEN a clear list-loading error MUST be displayed
- AND existing filter selections SHOULD remain visible for correction or retry
