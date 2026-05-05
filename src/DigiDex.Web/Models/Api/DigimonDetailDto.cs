using System.Text.Json.Serialization;

namespace DigiDex.Web.Models.Api;

public sealed record class DigimonDetailDto(
    [property: JsonPropertyName("id")] int? Id,
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("xAntibody")] bool? XAntibody,
    [property: JsonPropertyName("images")] IReadOnlyList<DigimonImageDto>? Images,
    [property: JsonPropertyName("levels")] IReadOnlyList<LevelDto>? Levels,
    [property: JsonPropertyName("types")] IReadOnlyList<TypeDto>? Types,
    [property: JsonPropertyName("attributes")] IReadOnlyList<AttributeDto>? Attributes,
    [property: JsonPropertyName("fields")] IReadOnlyList<FieldDto>? Fields,
    [property: JsonPropertyName("releaseDate")] string? ReleaseDate,
    [property: JsonPropertyName("descriptions")] IReadOnlyList<DigimonDescriptionDto>? Descriptions,
    [property: JsonPropertyName("skills")] IReadOnlyList<SkillDto>? Skills,
    [property: JsonPropertyName("priorEvolutions")] IReadOnlyList<EvolutionDto>? PriorEvolutions,
    [property: JsonPropertyName("nextEvolutions")] IReadOnlyList<EvolutionDto>? NextEvolutions);

public sealed record class DigimonImageDto(
    [property: JsonPropertyName("href")] string? Href,
    [property: JsonPropertyName("transparent")] bool? Transparent);

public sealed record class LevelDto(
    [property: JsonPropertyName("id")] int? Id,
    [property: JsonPropertyName("level")] string? Level,
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("href")] string? Href);

public sealed record class TypeDto(
    [property: JsonPropertyName("id")] int? Id,
    [property: JsonPropertyName("type")] string? Type,
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("href")] string? Href);

public sealed record class AttributeDto(
    [property: JsonPropertyName("id")] int? Id,
    [property: JsonPropertyName("attribute")] string? Attribute,
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("href")] string? Href);

public sealed record class FieldDto(
    [property: JsonPropertyName("id")] int? Id,
    [property: JsonPropertyName("field")] string? Field,
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("image")] string? Image,
    [property: JsonPropertyName("href")] string? Href);

public sealed record class DigimonDescriptionDto(
    [property: JsonPropertyName("origin")] string? Origin,
    [property: JsonPropertyName("language")] string? Language,
    [property: JsonPropertyName("description")] string? Description);

public sealed record class SkillDto(
    [property: JsonPropertyName("id")] int? Id,
    [property: JsonPropertyName("skill")] string? Skill,
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("translation")] string? Translation,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("href")] string? Href);

public sealed record class EvolutionDto(
    [property: JsonPropertyName("id")] int? Id,
    [property: JsonPropertyName("digimon")] string? Digimon,
    [property: JsonPropertyName("condition")] string? Condition,
    [property: JsonPropertyName("image")] string? Image,
    [property: JsonPropertyName("url")] string? Url);
