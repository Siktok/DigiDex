namespace DigiDex.Web.Models.ViewModels;

public sealed record class DigimonEvolutionViewModel(
    int Id,
    string Name,
    string? Condition,
    string? ImageUrl);
