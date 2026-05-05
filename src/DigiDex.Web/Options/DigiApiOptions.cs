namespace DigiDex.Web.Options;

public sealed class DigiApiOptions
{
    public const string SectionName = "DigiApi";

    public string BaseUrl { get; init; } = "https://digi-api.com/api/v1/";

    public int DefaultPageSize { get; init; } = 20;
}
