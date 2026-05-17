using System.Diagnostics;
using System.Net;

namespace DigiDex.Web.E2ETests;

public sealed class DigiDexWebAppFixture : IAsyncLifetime
{
    private const string DefaultBaseUrl = "http://127.0.0.1:5064";
    private static readonly TimeSpan StartupTimeout = TimeSpan.FromSeconds(45);

    private Process? _process;
    private bool _startedProcess;

    public string BaseUrl { get; private set; } = DefaultBaseUrl;

    public async Task InitializeAsync()
    {
        var configuredBaseUrl = Environment.GetEnvironmentVariable("DIGIDEX_BASE_URL");
        if (!string.IsNullOrWhiteSpace(configuredBaseUrl))
        {
            BaseUrl = configuredBaseUrl.TrimEnd('/');
            await WaitUntilAvailableAsync(BaseUrl);
            return;
        }

        if (await IsAvailableAsync(BaseUrl))
        {
            return;
        }

        var repositoryRoot = FindRepositoryRoot();
        var projectPath = Path.Combine(repositoryRoot, "src", "DigiDex.Web", "DigiDex.Web.csproj");

        _process = Process.Start(new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run --project \"{projectPath}\" --no-launch-profile --urls {DefaultBaseUrl}",
            WorkingDirectory = repositoryRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        });

        if (_process is null)
        {
            throw new InvalidOperationException("No se pudo iniciar DigiDex para las pruebas E2E.");
        }

        _startedProcess = true;
        await WaitUntilAvailableAsync(BaseUrl);
    }

    public Task DisposeAsync()
    {
        if (_startedProcess && _process is { HasExited: false })
        {
            _process.Kill(entireProcessTree: true);
            _process.Dispose();
        }

        return Task.CompletedTask;
    }

    private static async Task WaitUntilAvailableAsync(string baseUrl)
    {
        using var httpClient = CreateProbeClient();
        var deadline = DateTimeOffset.UtcNow.Add(StartupTimeout);

        while (DateTimeOffset.UtcNow < deadline)
        {
            if (await IsAvailableAsync(baseUrl, httpClient))
            {
                return;
            }

            await Task.Delay(500);
        }

        throw new TimeoutException($"DigiDex no respondio en {baseUrl} antes de {StartupTimeout.TotalSeconds} segundos.");
    }

    private static Task<bool> IsAvailableAsync(string baseUrl)
    {
        using var httpClient = CreateProbeClient();
        return IsAvailableAsync(baseUrl, httpClient);
    }

    private static HttpClient CreateProbeClient()
    {
        return new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(2)
        };
    }

    private static async Task<bool> IsAvailableAsync(string baseUrl, HttpClient httpClient)
    {
        try
        {
            using var response = await httpClient.GetAsync(baseUrl);
            return response.StatusCode is HttpStatusCode.OK;
        }
        catch (HttpRequestException)
        {
            return false;
        }
        catch (TaskCanceledException)
        {
            return false;
        }
    }

    private static string FindRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);

        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "DigiDex.slnx")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("No se encontro la raiz del repositorio DigiDex.");
    }
}
