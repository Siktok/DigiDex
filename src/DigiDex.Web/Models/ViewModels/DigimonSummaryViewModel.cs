namespace DigiDex.Web.Models.ViewModels;

public sealed record class DigimonSummaryViewModel(
    int Id,
    string Name,
    string? Href,
    string? ImageUrl);
