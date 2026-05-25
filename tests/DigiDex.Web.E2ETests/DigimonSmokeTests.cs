using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace DigiDex.Web.E2ETests;

public sealed class DigimonSmokeTests(DigiDexWebAppFixture app) : PageTest, IClassFixture<DigiDexWebAppFixture>
{
    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions
        {
            BaseURL = app.BaseUrl,
            ViewportSize = new ViewportSize
            {
                Width = 1280,
                Height = 900
            }
        };
    }

    [Fact]
    public async Task UserCanSearchAndOpenDigimonDetails()
    {
        await Page.GotoAsync("/");

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Digimon", Exact = true })).ToBeVisibleAsync();

        var firstDigimonLink = Page
            .GetByRole(AriaRole.Link, new() { NameRegex = new Regex("Ver detalle de", RegexOptions.IgnoreCase) })
            .First;

        await Expect(firstDigimonLink).ToBeVisibleAsync(new() { Timeout = 30_000 });

        await Page.GetByLabel("Buscar por nombre").FillAsync("Agumon");

        var agumonLink = Page.GetByRole(AriaRole.Link, new() { Name = "Ver detalle de Agumon", Exact = true });
        await Expect(agumonLink).ToBeVisibleAsync(new() { Timeout = 30_000 });

        await agumonLink.ClickAsync();

        await Expect(Page).ToHaveURLAsync(new Regex("/digimon/\\d+$"));
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Agumon" })).ToBeVisibleAsync();
        await Expect(Page.GetByText("Niveles")).ToBeVisibleAsync();
    }

    [Fact]
    public async Task UserCanSeeSupportedFiltersWithoutTypeFilter()
    {
        await Page.GotoAsync("/");

        await Expect(Page.GetByLabel("Filtrar por nivel")).ToBeVisibleAsync(new() { Timeout = 30_000 });
        await Expect(Page.GetByLabel("Filtrar por atributo")).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("Filtrar por X-Antibody")).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("Filtrar por tipo")).ToHaveCountAsync(0);
    }

    [Fact]
    public async Task UserCanCombineSearchAndFiltersThenKeepCriteriaInFullListMode()
    {
        await Page.GotoAsync("/");

        await Expect(Page.GetByLabel("Filtrar por nivel")).ToBeEnabledAsync(new() { Timeout = 30_000 });

        await Page.GetByLabel("Buscar por nombre").FillAsync("Agumon");
        await Page.GetByLabel("Filtrar por nivel").SelectOptionAsync("Child");
        await Page.GetByLabel("Filtrar por X-Antibody").SelectOptionAsync("false");

        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Ver detalle de Agumon", Exact = true })).ToBeVisibleAsync(new() { Timeout = 30_000 });
        await Expect(Page.GetByText("Pagina 1", new() { Exact = true })).ToBeVisibleAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Lista completa", Exact = true }).ClickAsync();

        await Expect(Page.GetByLabel("Filtrar por nivel")).ToHaveValueAsync("Child");
        await Expect(Page.GetByLabel("Filtrar por X-Antibody")).ToHaveValueAsync("false");
        var filteredAgumonLink = Page.GetByRole(AriaRole.Link, new() { Name = "Ver detalle de Agumon", Exact = true });
        await Expect(filteredAgumonLink).ToBeVisibleAsync(new() { Timeout = 30_000 });

        await filteredAgumonLink.ClickAsync();

        await Expect(Page).ToHaveURLAsync(new Regex("/digimon/\\d+$"));
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Agumon" })).ToBeVisibleAsync();
    }
}
