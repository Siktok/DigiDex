namespace DigiDex.Web.Models.ViewModels;

public sealed record class DigimonDetailViewModel(
    int Id,
    string Name,
    bool XAntibody,
    IReadOnlyList<DigimonImageViewModel> Images,
    IReadOnlyList<string> Levels,
    IReadOnlyList<string> Types,
    IReadOnlyList<string> Attributes,
    IReadOnlyList<string> Fields,
    string? ReleaseDate,
    string? PrimaryDescription,
    IReadOnlyList<string> Skills,
    IReadOnlyList<DigimonEvolutionViewModel> PriorEvolutions,
    IReadOnlyList<DigimonEvolutionViewModel> NextEvolutions)
{
    public string? PrimaryImageUrl => Images.FirstOrDefault()?.Url;
}
