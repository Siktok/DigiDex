using DigiDex.Web.Components;
using DigiDex.Web.Options;
using DigiDex.Web.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services
    .AddOptions<DigiApiOptions>()
    .Bind(builder.Configuration.GetSection(DigiApiOptions.SectionName))
    .Validate(options => Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out _), "DigiApi:BaseUrl must be an absolute URL.")
    .Validate(options => options.DefaultPageSize > 0, "DigiApi:DefaultPageSize must be greater than zero.")
    .ValidateOnStart();

builder.Services.AddMemoryCache();

builder.Services.AddHttpClient<IDigiApiClient, DigiApiClient>((serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<DigiApiOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl, UriKind.Absolute);
    client.Timeout = TimeSpan.FromSeconds(15);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
