using DigiDex.Web.Models.ViewModels;

namespace DigiDex.Web.Services;

public interface IDigiApiClient
{
    Task<DigimonListViewModel> GetDigimonPageAsync(
        int page,
        int pageSize,
        string? name = null,
        string? level = null,
        string? attribute = null,
        bool? xAntibody = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DigimonSummaryViewModel>> GetAllDigimonAsync(
        string? name = null,
        string? level = null,
        string? attribute = null,
        bool? xAntibody = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DigimonFilterOptionViewModel>> GetLevelOptionsAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DigimonFilterOptionViewModel>> GetAttributeOptionsAsync(
        CancellationToken cancellationToken = default);

    Task<DigimonDetailViewModel?> GetDigimonByIdAsync(
        int id,
        CancellationToken cancellationToken = default);
}
