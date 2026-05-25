using System.Net;
using System.Text.Json;
using DigiDex.Web.Services;
using Microsoft.Extensions.Caching.Memory;

namespace DigiDex.Web.Tests;

public sealed class DigiApiClientFilterTests
{
    [Fact]
    public async Task GetLevelOptionsAsync_MapsReferenceFieldsToFilterOptions()
    {
        var handler = new StubHttpMessageHandler(request =>
        {
            Assert.Equal("/api/v1/level", request.RequestUri?.AbsolutePath);

            return JsonResponse(new
            {
                name = "Level",
                description = "Digimon levels",
                fields = new[]
                {
                    new { id = 4, name = "Child", href = "https://digi-api.com/api/v1/level/4" },
                    new { id = 5, name = "Adult", href = "https://digi-api.com/api/v1/level/5" }
                },
                pageable = new { currentPage = 0, nextPage = (string?)null, totalPages = 1 }
            });
        });

        var client = CreateClient(handler);

        var options = await client.GetLevelOptionsAsync(CancellationToken.None);

        Assert.Collection(
            options,
            option =>
            {
                Assert.Equal(4, option.Id);
                Assert.Equal("Child", option.Name);
                Assert.Equal("https://digi-api.com/api/v1/level/4", option.Href);
            },
            option =>
            {
                Assert.Equal(5, option.Id);
                Assert.Equal("Adult", option.Name);
                Assert.Equal("https://digi-api.com/api/v1/level/5", option.Href);
            });
    }

    [Fact]
    public async Task GetLevelOptionsAsync_MapsReferenceFieldsFromContentEnvelope()
    {
        var handler = new StubHttpMessageHandler(_ => JsonResponse(new
        {
            content = new
            {
                name = "Level",
                description = "Digimon levels",
                fields = new[]
                {
                    new { id = 4, name = "Child", href = "https://digi-api.com/api/v1/level/4" }
                }
            },
            pageable = new { currentPage = 0, nextPage = (string?)null, totalPages = 1 }
        }));

        var client = CreateClient(handler);

        var options = await client.GetLevelOptionsAsync(CancellationToken.None);

        var option = Assert.Single(options);
        Assert.Equal(4, option.Id);
        Assert.Equal("Child", option.Name);
    }

    [Fact]
    public async Task GetAttributeOptionsAsync_FollowsNextPageUntilItEnds()
    {
        var handler = new StubHttpMessageHandler(request =>
        {
            var query = request.RequestUri?.Query ?? string.Empty;

            if (query.Contains("page=1", StringComparison.Ordinal))
            {
                return JsonResponse(new
                {
                    name = "Attribute",
                    description = "Digimon attributes",
                    fields = new[]
                    {
                        new { id = 2, name = "Data", href = "https://digi-api.com/api/v1/attribute/2" }
                    },
                    pageable = new { currentPage = 1, nextPage = (string?)null, totalPages = 99 }
                });
            }

            return JsonResponse(new
            {
                name = "Attribute",
                description = "Digimon attributes",
                fields = new[]
                {
                    new { id = 1, name = "Vaccine", href = "https://digi-api.com/api/v1/attribute/1" }
                },
                pageable = new { currentPage = 0, nextPage = "https://digi-api.com/api/v1/attribute?page=1", totalPages = 1 }
            });
        });

        var client = CreateClient(handler);

        var options = await client.GetAttributeOptionsAsync(CancellationToken.None);

        Assert.Equal(new[] { "Vaccine", "Data" }, options.Select(option => option.Name));
        Assert.Equal(2, handler.Requests.Count);
    }

    [Fact]
    public async Task GetAttributeOptionsAsync_ReusesCachedReferenceOptions()
    {
        var handler = new StubHttpMessageHandler(_ => JsonResponse(new
        {
            content = new
            {
                name = "Attribute",
                description = "Digimon attributes",
                fields = new[]
                {
                    new { id = 1, name = "Vaccine", href = "https://digi-api.com/api/v1/attribute/1" }
                }
            },
            pageable = new { currentPage = 0, nextPage = (string?)null, totalPages = 1 }
        }));

        var client = CreateClient(handler);

        var firstLoad = await client.GetAttributeOptionsAsync(CancellationToken.None);
        var secondLoad = await client.GetAttributeOptionsAsync(CancellationToken.None);

        Assert.Equal(new[] { "Vaccine" }, firstLoad.Select(option => option.Name));
        Assert.Same(firstLoad, secondLoad);
        Assert.Single(handler.Requests);
    }

    [Theory]
    [InlineData("Child", "Vaccine", true, "xAntibody=true")]
    [InlineData("Adult", "Data", false, "xAntibody=false")]
    public async Task GetDigimonPageAsync_SendsFilterQueryValuesUnchanged(
        string level,
        string attribute,
        bool xAntibody,
        string expectedXAntibodyQuery)
    {
        var handler = new StubHttpMessageHandler(request =>
        {
            var uri = request.RequestUri?.ToString() ?? string.Empty;

            Assert.Contains($"level={level}", uri, StringComparison.Ordinal);
            Assert.Contains($"attribute={attribute}", uri, StringComparison.Ordinal);
            Assert.Contains(expectedXAntibodyQuery, uri, StringComparison.Ordinal);

            return JsonResponse(new
            {
                content = Array.Empty<object>(),
                pageable = new { currentPage = 0, totalPages = 0, totalElements = 0 }
            });
        });

        var client = CreateClient(handler);

        await client.GetDigimonPageAsync(
            0,
            20,
            level: level,
            attribute: attribute,
            xAntibody: xAntibody,
            cancellationToken: CancellationToken.None);
    }

    private static DigiApiClient CreateClient(StubHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://digi-api.com/api/v1/")
        };

        return new DigiApiClient(httpClient, new MemoryCache(new MemoryCacheOptions()));
    }

    private static HttpResponseMessage JsonResponse(object value)
    {
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(value))
        };
    }

    private sealed class StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) : HttpMessageHandler
    {
        public List<HttpRequestMessage> Requests { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(request);
            return Task.FromResult(responder(request));
        }
    }
}
