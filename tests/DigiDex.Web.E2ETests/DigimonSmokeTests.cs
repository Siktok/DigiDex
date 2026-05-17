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
}
