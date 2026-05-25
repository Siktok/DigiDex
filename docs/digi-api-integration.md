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
- `GET /level`: opciones validas para el filtro de nivel.
- `GET /attribute`: opciones validas para el filtro de atributo.

Los DTOs se basan en la forma real verificada de Digi-API. En los endpoints de metadatos se admite tanto la forma directa `{ name, description, fields[], pageable }` como la forma observada con envoltorio `{ content: { name, description, fields[] }, pageable }`, porque la paginacion publicada por Digi-API no siempre coincide con lo esperado.

## Decision tomada

`IDigiApiClient` expone metodos asincronos orientados a la UI:

- `GetDigimonPageAsync`
- `GetAllDigimonAsync`
- `GetDigimonByIdAsync`
- `GetLevelOptionsAsync`
- `GetAttributeOptionsAsync`

`DigiApiClient` usa `HttpClient` tipado, `System.Text.Json`, `IHttpClientFactory` e `IMemoryCache`.

La busqueda por nombre se dispara desde `DigimonSearchBox` mientras el usuario escribe. El componente recibe el termino actual mediante binding Razor y aplica un pequeno debounce para evitar una llamada por cada pulsacion inmediata.

El modo de lista completa usa `GetAllDigimonAsync`, que recorre paginas de Digi-API con un tamano mayor y un limite defensivo de paginas para evitar bucles si la paginacion remota cambia.

Los filtros de nivel, atributo y X-Antibody se envian a `GET /digimon` con los nombres originales de Digi-API, por ejemplo `Child`, `Adult` o `Vaccine`. No se introducen alias como `Rookie`, porque eso romperia el contrato real del endpoint.

Las opciones de nivel y atributo se cargan desde Digi-API siguiendo `nextPage` con un limite defensivo de paginas. No se confia ciegamente en `totalPages`, ya que la metadata puede reportar paginacion inconsistente.

## Manejo de errores

- `404` en detalle devuelve `null` para representar "no encontrado".
- Errores HTTP, de red o timeout se convierten en `DigiApiException` con mensajes aptos para UI.
- Los componentes capturan `DigiApiException` y muestran estados de error sin exponer stack traces.
- Si fallan las opciones de filtro, la barra muestra un error claro y deshabilita los selectores de filtro. El listado sigue siendo usable sin permitir valores manuales de fallback.

## Cache

El detalle por ID se guarda en `IMemoryCache` durante 30 minutos. Las opciones de nivel y atributo se cachean durante 6 horas porque son metadata repetida y de baja variacion. El listado, la lista completa y la busqueda no se cachean todavia para evitar resultados obsoletos durante el MVP.

## Alternativas descartadas

- Cachear busquedas: no hay evidencia de problema de rendimiento.
- Llamar HTTP desde Razor: acoplaria UI y transporte.
- Generar cliente OpenAPI: seria mas pesado para el alcance actual.
- Hardcodear niveles y atributos: seria mas simple, pero se desalinearia si Digi-API cambia nombres o agrega opciones.

## Mantenimiento

Si la OpenAPI o Digi-API cambian, actualizar primero los DTOs en `Models/Api`, despues el mapeo en `DigiApiClient`, y por ultimo los ViewModels si la UI necesita nuevos datos.
