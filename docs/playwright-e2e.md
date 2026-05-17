# Playwright E2E

## Objetivo

La suite `tests/DigiDex.Web.E2ETests` valida un flujo basico de usuario en un navegador real: abrir DigiDex, cargar el listado, buscar un Digimon y navegar a su detalle.

Playwright controla el navegador desde C#. En este proyecto se usa `Microsoft.Playwright.Xunit` para integrarlo con `dotnet test`.

## Archivos afectados

- `tests/DigiDex.Web.E2ETests/DigiDex.Web.E2ETests.csproj`
- `tests/DigiDex.Web.E2ETests/DigiDexWebAppFixture.cs`
- `tests/DigiDex.Web.E2ETests/DigimonSmokeTests.cs`
- `DigiDex.slnx`

## Decision tomada

Se usa Playwright .NET con xUnit y Digi-API real. El test arranca DigiDex localmente en `http://127.0.0.1:5064` si no se define `DIGIDEX_BASE_URL`.

Tambien puede ejecutarse contra una app ya levantada:

```powershell
$env:DIGIDEX_BASE_URL="http://127.0.0.1:5064"
dotnet test tests/DigiDex.Web.E2ETests
```

## Motivo

Esta opcion mantiene el flujo dentro de .NET, evita introducir Node.js y permite aprender Playwright con una prueba pequena que recorre la UI real.

## Alternativas descartadas

- Playwright con Node.js: anade otra herramienta y otro sistema de configuracion para un MVP .NET.
- Datos falsos en tests: harian la prueba mas estable, pero se eligio validar contra Digi-API real.
- Cobertura E2E amplia: seria mas costosa de mantener para una primera incorporacion.

## Uso

Restaurar y compilar:

```powershell
dotnet restore
dotnet build
```

Instalar los navegadores que usa Playwright:

```powershell
powershell -ExecutionPolicy Bypass -File tests/DigiDex.Web.E2ETests/bin/Debug/net10.0/playwright.ps1 install
```

Si tienes PowerShell 7 instalado, tambien puedes usar `pwsh` en lugar de `powershell`.

Ejecutar tests:

```powershell
dotnet test
```

Ejecutar solo la suite E2E:

```powershell
dotnet test tests/DigiDex.Web.E2ETests
```

### PowerShell frente a CMD

En PowerShell las variables de entorno se definen con `$env:`:

```powershell
$env:HEADED="1"
$env:DIGIDEX_BASE_URL="http://127.0.0.1:5064"
dotnet test tests/DigiDex.Web.E2ETests
```

En CMD se definen con `set`:

```cmd
set HEADED=1
set DIGIDEX_BASE_URL=http://127.0.0.1:5064
dotnet test tests\DigiDex.Web.E2ETests
```

`HEADED=1` abre el navegador visible, pero el test sigue ejecutandose rapido.

Para depurar paso a paso, usar `PWDEBUG=1`:

```cmd
set PWDEBUG=1
set DIGIDEX_BASE_URL=http://127.0.0.1:5064
dotnet test tests\DigiDex.Web.E2ETests
```

`PWDEBUG=1` abre el Playwright Inspector y permite avanzar accion por accion.

## Funcion de `DigiDexWebAppFixture`

`DigimonSmokeTests` describe lo que hace el usuario en el navegador. `DigiDexWebAppFixture` prepara la aplicacion para que ese test tenga una URL real contra la que trabajar.

La fixture cumple estas responsabilidades:

- Usa `DIGIDEX_BASE_URL` si se proporciona desde la terminal o desde CI.
- Si no se proporciona, intenta reutilizar `http://127.0.0.1:5064` si ya hay una app levantada.
- Si no hay app levantada, ejecuta `dotnet run --project src/DigiDex.Web --no-launch-profile --urls http://127.0.0.1:5064`.
- Espera hasta 45 segundos a que DigiDex responda `200 OK`.
- Si la propia fixture arranco el proceso, lo cierra al finalizar los tests.

Esto hace que `dotnet test` funcione tanto en local como en GitHub Actions sin levantar la aplicacion manualmente.

En el test, la fixture se conecta asi:

```csharp
public sealed class DigimonSmokeTests(DigiDexWebAppFixture app)
    : PageTest, IClassFixture<DigiDexWebAppFixture>
```

Y se usa aqui:

```csharp
BaseURL = app.BaseUrl
```

Gracias a eso, el test puede navegar con rutas relativas:

```csharp
await Page.GotoAsync("/");
```

## Selectores y esperas

Los tests deben preferir selectores accesibles:

```csharp
Page.GetByRole(AriaRole.Heading, new() { Name = "Digimon", Exact = true })
Page.GetByLabel("Buscar por nombre")
Page.GetByRole(AriaRole.Link, new() { Name = "Ver detalle de Agumon", Exact = true })
```

Cuando un texto puede coincidir con variantes, usar `Exact = true`. Por ejemplo, `Agumon` tambien coincide parcialmente con `Agumon (Black)` si el selector no es exacto.

Playwright ya espera automaticamente con `Expect`:

```csharp
await Expect(locator).ToBeVisibleAsync();
```

Evitar esperas fijas como `Thread.Sleep` o `Task.Delay` salvo para depuracion puntual.

## GitHub Actions

El workflow `.github/workflows/playwright-e2e.yml` ejecuta los tests E2E automaticamente en pull requests hacia `main` y pushes a `main`.

El workflow realiza estos pasos:

```text
checkout
setup .NET 10
dotnet restore
dotnet build --no-restore
playwright.ps1 install --with-deps
dotnet test tests/DigiDex.Web.E2ETests --no-build
```

Si los tests pasan, GitHub muestra el check verde en la pull request. Si fallan, muestra el check rojo y deja los logs disponibles en la ejecucion del workflow.

Para impedir merges con checks fallidos hay que configurar una regla de proteccion en GitHub:

```text
Settings -> Branches o Rulesets -> Require status checks to pass before merging
```

Despues se debe seleccionar el check `Playwright E2E / Build and run Playwright E2E`.

## Mantenimiento

Los tests E2E dependen de Digi-API real. Si hay latencia, caidas o cambios en los datos externos, el test puede fallar aunque DigiDex no haya cambiado.

Los selectores deben preferir roles accesibles, etiquetas y texto visible antes que clases CSS. Esto hace que el test compruebe comportamiento de usuario y no detalles internos de estilo.
