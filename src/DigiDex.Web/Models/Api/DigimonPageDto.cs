using System.Text.Json.Serialization;

namespace DigiDex.Web.Models.Api;

public sealed record class DigimonPageDto(
    [property: JsonPropertyName("content")] IReadOnlyList<DigimonSummaryDto>? Content,
    [property: JsonPropertyName("pageable")] PageableDto? Pageable);

public sealed record class PageableDto(
    [property: JsonPropertyName("currentPage")] int? CurrentPage,
    [property: JsonPropertyName("elementsOnPage")] int? ElementsOnPage,
    [property: JsonPropertyName("totalElements")] int? TotalElements,
    [property: JsonPropertyName("totalPages")] int? TotalPages,
    [property: JsonPropertyName("previousPage")] string? PreviousPage,
    [property: JsonPropertyName("nextPage")] string? NextPage);

public sealed record class DigimonSummaryDto(
    [property: JsonPropertyName("id")] int? Id,
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("href")] string? Href,
    [property: JsonPropertyName("image")] string? Image);
