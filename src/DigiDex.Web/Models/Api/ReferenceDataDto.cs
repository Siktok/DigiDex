using System.Text.Json.Serialization;

namespace DigiDex.Web.Models.Api;

public sealed record class ReferenceDataDto(
    string? Name,
    string? Description,
    IReadOnlyList<ReferenceFieldDto>? Fields,
    ReferencePageableDto? Pageable);

public sealed record class ReferenceDataEnvelopeDto(
    ReferenceDataDto? Content,
    ReferencePageableDto? Pageable);

public sealed record class ReferenceFieldDto(
    int? Id,
    string? Name,
    string? Href);

public sealed record class ReferencePageableDto(
    int? CurrentPage,
    [property: JsonPropertyName("nextPage")] string? NextPage,
    int? TotalPages);
