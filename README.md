# DigiDex

DigiDex es una aplicacion web de aprendizaje para consultar informacion de Digimon usando la API publica [Digi-API](https://digi-api.com/api/v1/).

El MVP esta construido con .NET 10, ASP.NET Core Blazor Web App e Interactive Server. No usa base de datos, autenticacion ni dependencias de arquitectura pesada.

## Requisitos

- .NET SDK 10
- Conexion a internet para consultar Digi-API

## Configuracion

La integracion con Digi-API se configura en `src/DigiDex.Web/appsettings.json`:

```json
{
  "DigiApi": {
    "BaseUrl": "https://digi-api.com/api/v1/",
    "DefaultPageSize": 20
  }
}
```

## Comandos utiles

Restaurar paquetes:

```bash
dotnet restore
```

Compilar:

```bash
dotnet build
```

Ejecutar:

```bash
dotnet run --project src/DigiDex.Web
```

Ejecutar tests:

```bash
dotnet test
```

Instalar navegadores de Playwright tras el primer build:

```bash
powershell -ExecutionPolicy Bypass -File tests/DigiDex.Web.E2ETests/bin/Debug/net10.0/playwright.ps1 install
```

Ejecutar los tests E2E contra una app ya levantada:

```powershell
$env:DIGIDEX_BASE_URL="http://127.0.0.1:5064"
dotnet test tests/DigiDex.Web.E2ETests
```

## Integracion continua

El workflow `Playwright E2E` de GitHub Actions ejecuta build y tests E2E en cada pull request hacia `main` y en cada push a `main`.

Si el workflow falla, GitHub marca la pull request con un check rojo. Para bloquear merges con tests fallidos, configura una regla de proteccion de rama o ruleset en GitHub y exige que el check `Playwright E2E / Build and run Playwright E2E` pase antes de fusionar.

## Estructura basica

```text
src/DigiDex.Web/
  Components/
    Digimon/
    Layout/
    Pages/
    Shared/
  Models/
    Api/
    ViewModels/
  Options/
  Services/
  wwwroot/
docs/
Context/digi-api.openapi.yaml
```

## Funcionalidades del MVP

- Listado paginado de Digimon como pantalla principal.
- Modo de tarjetas paginadas y modo de lista completa.
- Busqueda dinamica por nombre mientras se escribe.
- Detalle de Digimon por ID.
- Imagenes, niveles, tipos, atributos, campos, habilidades y evoluciones cuando Digi-API los devuelve.
- Estados de carga, error y resultados vacios.
- Interfaz responsive con controles alineados, tarjetas visuales y lista compacta.
