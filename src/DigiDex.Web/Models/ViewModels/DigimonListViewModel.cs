namespace DigiDex.Web.Models.ViewModels;

public sealed record class DigimonListViewModel(
    IReadOnlyList<DigimonSummaryViewModel> Items,
    int CurrentPage,
    int PageSize,
    int TotalElements,
    int TotalPages)
{
    public bool HasItems => Items.Count > 0;

    public bool HasPreviousPage => CurrentPage > 0;

    public bool HasNextPage => TotalPages == 0 ? Items.Count == PageSize : CurrentPage + 1 < TotalPages;
}
