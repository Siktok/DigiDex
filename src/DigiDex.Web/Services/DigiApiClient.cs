using System.Net;
using System.Text.Json;
using DigiDex.Web.Models.Api;
using DigiDex.Web.Models.ViewModels;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;

namespace DigiDex.Web.Services;

public sealed class DigiApiClient : IDigiApiClient
{
    private const int CompleteListPageSize = 100;
    private const int CompleteListMaxPages = 100;
    private const int ReferenceDataMaxPages = 25;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly TimeSpan DetailCacheDuration = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan ReferenceDataCacheDuration = TimeSpan.FromHours(6);

    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;

    public DigiApiClient(HttpClient httpClient, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _cache = cache;
    }

    public async Task<DigimonListViewModel> GetDigimonPageAsync(
        int page,
        int pageSize,
        string? name = null,
        string? level = null,
        string? attribute = null,
        bool? xAntibody = null,
        CancellationToken cancellationToken = default)
    {
        if (page < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(page), "Page must be zero or greater.");
        }

        if (pageSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");
        }

        var query = new Dictionary<string, string?>
        {
            ["page"] = page.ToString(),
            ["pageSize"] = pageSize.ToString()
        };

        AddOptionalQuery(query, "name", name);
        AddOptionalQuery(query, "level", level);
        AddOptionalQuery(query, "attribute", attribute);

        if (xAntibody is not null)
        {
            query["xAntibody"] = xAntibody.Value.ToString().ToLowerInvariant();
        }

        var requestUri = QueryHelpers.AddQueryString("digimon", query);
        var dto = await SendAndReadAsync<DigimonPageDto>(requestUri, cancellationToken);

        return MapPage(dto, pageSize);
    }

    public async Task<IReadOnlyList<DigimonSummaryViewModel>> GetAllDigimonAsync(
        string? name = null,
        string? level = null,
        string? attribute = null,
        bool? xAntibody = null,
        CancellationToken cancellationToken = default)
    {
        var results = new List<DigimonSummaryViewModel>();
        var currentPage = 0;

        while (currentPage < CompleteListMaxPages)
        {
            var page = await GetDigimonPageAsync(
                currentPage,
                CompleteListPageSize,
                name,
                level,
                attribute,
                xAntibody,
                cancellationToken);

            results.AddRange(page.Items);

            if (!page.HasNextPage)
            {
                break;
            }

            currentPage++;
        }

        return results
            .DistinctBy(digimon => digimon.Id)
            .OrderBy(digimon => digimon.Id)
            .ToArray();
    }

    public Task<IReadOnlyList<DigimonFilterOptionViewModel>> GetLevelOptionsAsync(
        CancellationToken cancellationToken = default)
    {
        return GetReferenceOptionsAsync("level", "digimon-level-options", cancellationToken);
    }

    public Task<IReadOnlyList<DigimonFilterOptionViewModel>> GetAttributeOptionsAsync(
        CancellationToken cancellationToken = default)
    {
        return GetReferenceOptionsAsync("attribute", "digimon-attribute-options", cancellationToken);
    }

    public async Task<DigimonDetailViewModel?> GetDigimonByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Digimon id must be greater than zero.");
        }

        var cacheKey = $"digimon-detail-{id}";

        if (_cache.TryGetValue(cacheKey, out DigimonDetailViewModel? cachedDetail))
        {
            return cachedDetail;
        }

        var response = await SendAsync($"digimon/{id}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new DigiApiException("No se pudo cargar el detalle del Digimon. Intentalo de nuevo.");
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var dto = await JsonSerializer.DeserializeAsync<DigimonDetailDto>(stream, JsonOptions, cancellationToken);

        if (dto is null)
        {
            throw new DigiApiException("La respuesta de Digi-API no pudo leerse correctamente.");
        }

        var detail = MapDetail(dto);
        _cache.Set(cacheKey, detail, DetailCacheDuration);

        return detail;
    }

    private static void AddOptionalQuery(IDictionary<string, string?> query, string key, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            query[key] = value.Trim();
        }
    }

    private async Task<IReadOnlyList<DigimonFilterOptionViewModel>> GetReferenceOptionsAsync(
        string endpoint,
        string cacheKey,
        CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<DigimonFilterOptionViewModel>? cachedOptions))
        {
            return cachedOptions ?? [];
        }

        var options = new List<DigimonFilterOptionViewModel>();
        var requestUri = endpoint;
        var pagesRead = 0;

        while (!string.IsNullOrWhiteSpace(requestUri) && pagesRead < ReferenceDataMaxPages)
        {
            var dto = await SendAndReadReferenceAsync(requestUri, cancellationToken);
            options.AddRange(MapReferenceFields(dto.Fields));

            requestUri = NormalizeNextPageUri(dto.Pageable?.NextPage);
            pagesRead++;
        }

        var mappedOptions = options
            .DistinctBy(option => option.Id)
            .OrderBy(option => option.Id)
            .ToArray();

        _cache.Set(cacheKey, mappedOptions, ReferenceDataCacheDuration);

        return mappedOptions;
    }

    private async Task<ReferenceDataDto> SendAndReadReferenceAsync(string requestUri, CancellationToken cancellationToken)
    {
        var response = await SendAsync(requestUri, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new DigiApiException("No se pudieron cargar las opciones de filtro. Inténtalo de nuevo.");
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var envelope = JsonSerializer.Deserialize<ReferenceDataEnvelopeDto>(json, JsonOptions);

        if (envelope?.Content is not null)
        {
            return envelope.Content.Pageable is null && envelope.Pageable is not null
                ? envelope.Content with { Pageable = envelope.Pageable }
                : envelope.Content;
        }

        var dto = JsonSerializer.Deserialize<ReferenceDataDto>(json, JsonOptions);

        return dto ?? throw new DigiApiException("La respuesta de opciones de filtro no pudo leerse correctamente.");
    }

    private static string? NormalizeNextPageUri(string? nextPage)
    {
        if (string.IsNullOrWhiteSpace(nextPage))
        {
            return null;
        }

        if (Uri.TryCreate(nextPage, UriKind.Absolute, out var absoluteUri))
        {
            return absoluteUri.PathAndQuery.TrimStart('/').Replace("api/v1/", string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        return nextPage.Trim().TrimStart('/');
    }

    private static IReadOnlyList<DigimonFilterOptionViewModel> MapReferenceFields(IReadOnlyList<ReferenceFieldDto>? fields)
    {
        return fields?
            .Where(field => field.Id is > 0 && !string.IsNullOrWhiteSpace(field.Name))
            .Select(field => new DigimonFilterOptionViewModel(
                field.Id!.Value,
                field.Name!.Trim(),
                string.IsNullOrWhiteSpace(field.Href) ? null : field.Href.Trim()))
            .ToArray() ?? [];
    }

    private async Task<T> SendAndReadAsync<T>(string requestUri, CancellationToken cancellationToken)
    {
        var response = await SendAsync(requestUri, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new DigiApiException("No se pudieron cargar los Digimon. Intentalo de nuevo.");
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var dto = await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions, cancellationToken);

        return dto ?? throw new DigiApiException("La respuesta de Digi-API no pudo leerse correctamente.");
    }

    private async Task<HttpResponseMessage> SendAsync(string requestUri, CancellationToken cancellationToken)
    {
        try
        {
            return await _httpClient.GetAsync(requestUri, cancellationToken);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new DigiApiException("Digi-API tardo demasiado en responder. Intentalo de nuevo.");
        }
        catch (HttpRequestException exception)
        {
            throw new DigiApiException("No se pudo conectar con Digi-API. Revisa tu conexion e intentalo de nuevo.", exception);
        }
        catch (TaskCanceledException exception)
        {
            throw new DigiApiException("Digi-API tardo demasiado en responder. Intentalo de nuevo.", exception);
        }
    }

    private static DigimonListViewModel MapPage(DigimonPageDto dto, int requestedPageSize)
    {
        var items = dto.Content?
            .Where(item => item.Id is > 0 && !string.IsNullOrWhiteSpace(item.Name))
            .Select(item => new DigimonSummaryViewModel(
                item.Id!.Value,
                item.Name!.Trim(),
                item.Href,
                item.Image))
            .ToArray() ?? [];

        var pageable = dto.Pageable;

        return new DigimonListViewModel(
            items,
            pageable?.CurrentPage ?? 0,
            requestedPageSize,
            pageable?.TotalElements ?? items.Length,
            pageable?.TotalPages ?? 0);
    }

    private static DigimonDetailViewModel MapDetail(DigimonDetailDto dto)
    {
        var id = dto.Id ?? 0;
        var name = string.IsNullOrWhiteSpace(dto.Name) ? $"Digimon #{id}" : dto.Name.Trim();

        return new DigimonDetailViewModel(
            id,
            name,
            dto.XAntibody ?? false,
            MapImages(dto.Images),
            MapNames(dto.Levels, level => level.Level ?? level.Name),
            MapNames(dto.Types, type => type.Type ?? type.Name),
            MapNames(dto.Attributes, attribute => attribute.Attribute ?? attribute.Name),
            MapNames(dto.Fields, field => field.Field ?? field.Name),
            dto.ReleaseDate,
            SelectPrimaryDescription(dto.Descriptions),
            MapNames(dto.Skills, skill => FormatSkill(skill)),
            MapEvolutions(dto.PriorEvolutions),
            MapEvolutions(dto.NextEvolutions));
    }

    private static IReadOnlyList<DigimonImageViewModel> MapImages(IReadOnlyList<DigimonImageDto>? images)
    {
        return images?
            .Where(image => !string.IsNullOrWhiteSpace(image.Href))
            .Select(image => new DigimonImageViewModel(image.Href!.Trim(), image.Transparent ?? false))
            .ToArray() ?? [];
    }

    private static IReadOnlyList<string> MapNames<T>(IReadOnlyList<T>? source, Func<T, string?> selector)
    {
        return source?
            .Select(selector)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray() ?? [];
    }

    private static string? SelectPrimaryDescription(IReadOnlyList<DigimonDescriptionDto>? descriptions)
    {
        return descriptions?
            .OrderByDescending(description => string.Equals(description.Language, "en_us", StringComparison.OrdinalIgnoreCase))
            .ThenByDescending(description => string.Equals(description.Origin, "reference_book", StringComparison.OrdinalIgnoreCase))
            .Select(description => description.Description)
            .FirstOrDefault(description => !string.IsNullOrWhiteSpace(description))
            ?.Trim();
    }

    private static string? FormatSkill(SkillDto skill)
    {
        var name = skill.Skill ?? skill.Name;

        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        return string.IsNullOrWhiteSpace(skill.Translation)
            ? name.Trim()
            : $"{name.Trim()} ({skill.Translation.Trim()})";
    }

    private static IReadOnlyList<DigimonEvolutionViewModel> MapEvolutions(IReadOnlyList<EvolutionDto>? evolutions)
    {
        return evolutions?
            .Where(evolution => evolution.Id is > 0 && !string.IsNullOrWhiteSpace(evolution.Digimon))
            .Select(evolution => new DigimonEvolutionViewModel(
                evolution.Id!.Value,
                evolution.Digimon!.Trim(),
                string.IsNullOrWhiteSpace(evolution.Condition) ? null : evolution.Condition.Trim(),
                evolution.Image))
            .ToArray() ?? [];
    }
}
