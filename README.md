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
- Busqueda dinamica por nombre mientras se escribe.
- Detalle de Digimon por ID.
- Imagenes, niveles, tipos, atributos, campos, habilidades y evoluciones cuando Digi-API los devuelve.
- Estados de carga, error y resultados vacios.
