# Integracion con Digi-API

## Objetivo

Centralizar el acceso a `https://digi-api.com/api/v1/` para que los componentes no hagan llamadas HTTP directamente.

## API base

La URL base se configura en `src/DigiDex.Web/appsettings.json`:

```json
{
  "DigiApi": {
    "BaseUrl": "https://digi-api.com/api/v1/",
    "DefaultPageSize": 20
  }
}
```

## Endpoints usados

- `GET /digimon`: listado paginado, busqueda por `name` y parametros preparados para `level`, `attribute` y `xAntibody`.
- `GET /digimon/{identifier}`: detalle por identificador numerico.

Los DTOs se basan en `Context/digi-api.openapi.yaml`.

## Decision tomada

`IDigiApiClient` expone metodos asincronos orientados a la UI:

- `GetDigimonPageAsync`
- `GetDigimonByIdAsync`

`DigiApiClient` usa `HttpClient` tipado, `System.Text.Json`, `IHttpClientFactory` e `IMemoryCache`.

## Manejo de errores

- `404` en detalle devuelve `null` para representar "no encontrado".
- Errores HTTP, de red o timeout se convierten en `DigiApiException` con mensajes aptos para UI.
- Los componentes capturan `DigiApiException` y muestran estados de error sin exponer stack traces.

## Cache

El detalle por ID se guarda en `IMemoryCache` durante 30 minutos. El listado y la busqueda no se cachean todavia para evitar resultados obsoletos durante el MVP.

## Alternativas descartadas

- Cachear busquedas: no hay evidencia de problema de rendimiento.
- Llamar HTTP desde Razor: acoplaria UI y transporte.
- Generar cliente OpenAPI: seria mas pesado para el alcance actual.

## Mantenimiento

Si la OpenAPI o Digi-API cambian, actualizar primero los DTOs en `Models/Api`, despues el mapeo en `DigiApiClient`, y por ultimo los ViewModels si la UI necesita nuevos datos.
