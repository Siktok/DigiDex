# Instrucciones para el asistente de IA

## Descripción del proyecto

Este repositorio contiene **DigiDex**, una enciclopedia de Digimon construida con **.NET 10** y **Blazor Web App**.

La aplicación consume datos de la API pública Digi-API:

- API base: `https://digi-api.com/api/v1/`
- Objetivo principal: navegar, buscar, filtrar e inspeccionar datos de Digimon.
- Estrategia inicial de renderizado: **Blazor Web App con Interactive Server**.
- Base de datos: **no es necesaria para el MVP**. La persistencia puede añadirse más adelante para favoritos, notas o equipos personalizados.

El proyecto debe construirse como un proyecto de aprendizaje limpio y mantenible, no como una aplicación empresarial sobrearquitecturada.

---

## Decisiones técnicas principales

Usar:

- .NET 10
- ASP.NET Core Blazor Web App
- Interactive Server como render mode para la primera versión
- C#
- Razor components
- `IHttpClientFactory`
- `System.Text.Json`
- `IMemoryCache`
- opciones fuertemente tipadas con `IOptions<T>`
- nullable reference types activados
- async/await para operaciones de I/O

No usar en el MVP salvo petición explícita:

- Entity Framework Core
- SQL Server
- autenticación
- Clean Architecture con demasiadas capas
- MediatR
- AutoMapper
- gestión de estado estilo Redux
- JavaScript salvo que sea estrictamente necesario
- Blazor WebAssembly como modelo inicial de hosting

---

## Alcance del producto

### Funcionalidades del MVP

La primera versión debe incluir:

- página de inicio
- página de listado de Digimon
- navegación paginada de Digimon
- búsqueda por nombre de Digimon
- página de detalle de Digimon
- componente reutilizable de tarjeta de Digimon
- estados de carga
- estados vacíos
- estados de error
- diseño responsive básico

### Funcionalidades posteriores

Estas funcionalidades pueden añadirse cuando el MVP sea estable:

- filtros por nivel
- filtros por atributo
- filtros por tipo
- filtro X-Antibody
- favoritos
- comparación entre Digimon
- Digimon vistos recientemente
- persistencia con SQLite
- EF Core
- notas personales
- equipos personalizados de Digimon
- mejora visual
- tests unitarios y de integración

---

## Estructura recomendada del proyecto

Usar esta estructura salvo que haya una razón fuerte para cambiarla:

```text
DigiDex/
├── src/
│   └── DigiDex.Web/
│       ├── Components/
│       │   ├── Layout/
│       │   ├── Pages/
│       │   │   ├── Home.razor
│       │   │   ├── DigimonList.razor
│       │   │   ├── DigimonDetails.razor
│       │   │   └── Favorites.razor
│       │   ├── Digimon/
│       │   │   ├── DigimonCard.razor
│       │   │   ├── DigimonGrid.razor
│       │   │   ├── DigimonFilters.razor
│       │   │   ├── DigimonSearchBox.razor
│       │   │   └── DigimonEvolutionChain.razor
│       │   └── Shared/
│       │       ├── LoadingSpinner.razor
│       │       ├── ErrorMessage.razor
│       │       └── EmptyState.razor
│       ├── Models/
│       │   ├── Api/
│       │   └── ViewModels/
│       ├── Options/
│       │   └── DigiApiOptions.cs
│       ├── Services/
│       │   ├── IDigiApiClient.cs
│       │   ├── DigiApiClient.cs
│       │   └── FavoritesService.cs
│       ├── wwwroot/
│       ├── appsettings.json
│       └── Program.cs
├── tests/
│   └── DigiDex.Web.Tests/
├── README.md
└── AGENTS.md
```

Si la estructura real del proyecto cambia, adaptarse a la estructura existente en lugar de imponer esta.

---

## Reglas de código

### Reglas generales

- Preferir código simple y explícito.
- Mantener cada archivo con una responsabilidad clara.
- Evitar abstracciones prematuras.
- Evitar añadir dependencias salvo que resuelvan un problema real.
- No introducir una base de datos hasta que el MVP funcione sin ella.
- No hardcodear URLs base de la API fuera de la configuración.
- No mezclar DTOs de la API directamente con el estado de UI si un ViewModel mejora la claridad.
- No ocultar excepciones silenciosamente.
- No devolver `null` cuando sea mejor usar un tipo de resultado explícito, una colección vacía o un estado de error.

### C#

- Usar nullable reference types correctamente.
- Preferir `record` o `record class` para DTOs y ViewModels inmutables.
- Preferir clases `sealed` salvo que la herencia sea necesaria.
- Usar `CancellationToken` en métodos async de servicios.
- Evitar `.Result`, `.Wait()` y sync-over-async.
- Usar pattern matching cuando mejore la claridad.
- Mantener los métodos públicos pequeños y legibles.
- Validar parámetros cuando el método forme parte de un límite de servicio.

### Async

Todas las llamadas a APIs externas deben ser asíncronas.

Correcto:

```csharp
public Task<DigimonDetailViewModel?> GetDigimonByIdAsync(
    int id,
    CancellationToken cancellationToken = default);
```

Incorrecto:

```csharp
public DigimonDetailViewModel GetDigimonById(int id);
```

### HTTP

- Usar `IHttpClientFactory`.
- Configurar `BaseAddress` en `Program.cs`.
- Configurar explícitamente el timeout.
- Manejar códigos HTTP no exitosos.
- Tratar `404` como un estado controlado de "not found".
- Tratar errores de red como estados de error visibles para el usuario.
- No exponer detalles crudos de excepciones en la UI.
- No hacer que los componentes sean responsables directos de llamadas HTTP de bajo nivel.

### JSON

- Usar `System.Text.Json`.
- Crear DTOs que reflejen la forma real de las respuestas de Digi-API.
- No asumir nombres de propiedades. Verificarlos contra respuestas reales de la API antes de depender de ellos.
- Usar `JsonPropertyName` cuando los nombres de propiedades de la API no coincidan con las convenciones de C#.

### Caché

- Usar `IMemoryCache` para datos repetidos de la API.
- Buenos candidatos para caché:
  - detalle de Digimon por ID
  - niveles
  - atributos
  - tipos
  - campos
- Evitar cachear resultados de búsqueda demasiado pronto salvo que haya un problema claro de rendimiento.
- La duración de caché debe ser conservadora y fácil de modificar.

---

## Reglas de Blazor

### Diseño de componentes

Los componentes deben ser pequeños y reutilizables.

Preferir:

- `DigimonCard`
- `DigimonGrid`
- `DigimonSearchBox`
- `DigimonFilters`
- `LoadingSpinner`
- `ErrorMessage`
- `EmptyState`

Evitar componentes grandes que obtengan datos, procesen datos y rendericen una UI compleja en el mismo archivo.

### Gestión de estado

Para el MVP:

- Mantener el estado local en la página o componente siempre que sea posible.
- Usar servicios scoped simples solo cuando el estado tenga que compartirse.
- No introducir librerías complejas de gestión de estado.

### Renderizado

- Modo inicial: Interactive Server.
- No cambiar a WebAssembly salvo petición explícita.
- Evitar JavaScript interop innecesario.
- Mantener los componentes responsive y accesibles.

### Comportamiento de UI

Toda página basada en datos debe manejar:

- estado de carga
- estado exitoso
- estado vacío
- estado de error

No dejar páginas en blanco mientras se cargan datos.

---

## Diseño del cliente de API

Crear una interfaz parecida a esta:

```csharp
public interface IDigiApiClient
{
    Task<DigimonListViewModel> GetDigimonPageAsync(
        int page,
        int pageSize,
        string? name = null,
        string? level = null,
        string? attribute = null,
        bool? xAntibody = null,
        CancellationToken cancellationToken = default);

    Task<DigimonDetailViewModel?> GetDigimonByIdAsync(
        int id,
        CancellationToken cancellationToken = default);
}
```

La implementación debe:

- construir query strings de forma segura
- deserializar respuestas de la API en DTOs
- mapear DTOs a ViewModels
- manejar errores de forma controlada
- usar caché cuando sea apropiado

---

## Reglas de manejo de errores

El asistente no debe generar código que esconda errores.

Usar estados explícitos como:

```csharp
public enum LoadState
{
    Idle,
    Loading,
    Success,
    Empty,
    Error
}
```

O usar flags simples a nivel de componente cuando sea apropiado:

```csharp
private bool _isLoading;
private string? _errorMessage;
private IReadOnlyList<DigimonSummaryViewModel> _digimon = [];
```

Los mensajes de error para el usuario deben ser claros, pero no excesivamente técnicos.

Correcto:

```text
No se pudieron cargar los Digimon. Inténtalo de nuevo.
```

Incorrecto:

```text
System.Net.Http.HttpRequestException: Connection refused...
```

---

## Reglas de testing

Los tests no son obligatorios para el primer prototipo pequeño, pero deben añadirse cuando el cliente de API y la lógica de mapeo sean estables.

Priorizar tests para:

- comportamiento del cliente de API
- mapeo de DTO a ViewModel
- manejo de errores
- lógica de paginación
- construcción de queries para búsqueda y filtros

Usar:

- xUnit
- bUnit para tests de componentes Blazor si el comportamiento de los componentes se vuelve complejo

No escribir tests frágiles que dependan directamente de la Digi-API real.

Preferir respuestas HTTP simuladas.

---

## Seguridad y fiabilidad

- No guardar secretos en el código fuente.
- No requerir API keys salvo que Digi-API las introduzca.
- No exponer stack traces a usuarios.
- No confiar ciegamente en datos externos de la API.
- Renderizar de forma segura cualquier texto de la API mostrado en la UI.
- Mantener dependencias actualizadas.
- No añadir paquetes sin comprobar primero si .NET ya ofrece la funcionalidad necesaria.

---

## Reglas de trabajo para el asistente

Cuando ayude con este repositorio, el asistente de IA debe seguir estas reglas:

1. Leer el código existente antes de proponer cambios grandes.
2. Preferir cambios incrementales.
3. Explicar la razón de los cambios arquitectónicos.
4. No sustituir código que funciona por una abstracción más grande salvo que mejore claramente el proyecto.
5. Mantener la aplicación compilable tras cada cambio significativo.
6. Al generar código, incluir la ruta esperada del archivo.
7. Al modificar código existente, mostrar solo las partes relevantes cambiadas salvo que sea necesario enseñar el archivo completo.
8. No inventar propiedades de respuesta de Digi-API. Verificar la forma real antes de programar contra ella.
9. No introducir EF Core, autenticación, CQRS, MediatR, AutoMapper ni arquitectura compleja sin aprobación explícita.
10. Si un requisito es ambiguo, hacer la suposición razonable más pequeña y declararla.
11. Si algo probablemente va a romperse, decirlo de forma directa.
12. Priorizar mantenibilidad sobre ingenio.
13. Priorizar un MVP terminado sobre una arquitectura impresionante pero incompleta.

---

## Documentación, versionado y changelog

Cada cambio realizado por el asistente debe dejar trazabilidad documental.

### Documentación en `docs/`

- Todo cambio funcional, técnico o arquitectónico debe documentarse dentro de la carpeta `docs/`.
- La documentación debe estar escrita en archivos Markdown (`.md`).
- Si la carpeta `docs/` no existe, el asistente debe crearla.
- No documentar únicamente en el chat. La explicación relevante debe quedar persistida en el repositorio.
- Cada documento debe tener un nombre claro y estable.

Ejemplos de archivos válidos:

```text
docs/api-client.md
docs/blazor-components.md
docs/project-architecture.md
docs/versioning.md
docs/digi-api-integration.md
```

Cada documento técnico debe incluir, cuando aplique:

- objetivo del cambio
- archivos afectados
- decisión tomada
- motivo de la decisión
- alternativas descartadas
- impacto sobre el proyecto
- instrucciones de uso o mantenimiento

No crear documentación excesiva para cambios triviales, pero sí dejar constancia de cualquier cambio que afecte a comportamiento, arquitectura, configuración, endpoints, modelos, servicios, componentes reutilizables, dependencias o flujo de ejecución.

### Versionado del programa

Cada cambio significativo debe incrementar la versión del programa.

El proyecto debe seguir **Semantic Versioning**:

```text
MAJOR.MINOR.PATCH
```

Reglas:

- Incrementar `PATCH` para correcciones internas, bugs, ajustes menores o refactors sin cambio funcional visible.
- Incrementar `MINOR` para funcionalidades nuevas compatibles hacia atrás.
- Incrementar `MAJOR` para cambios incompatibles, rediseños importantes o cambios que rompan comportamiento existente.
- No cambiar la versión por modificaciones puramente experimentales que no queden integradas.
- Si existe un archivo central de versión, actualizarlo.
- Si todavía no existe un archivo central de versión, proponer uno antes de dispersar la versión en varios lugares.

Ubicaciones preferidas para versionado, en orden:

1. `Directory.Build.props`
2. archivo `.csproj` principal
3. `appsettings.json`, solo si la versión se muestra en UI o se necesita en runtime

Evitar duplicar la versión en varios archivos salvo que haya una razón clara.

### `CHANGELOG.md`

Todo cambio significativo debe reflejarse en `CHANGELOG.md`.

El archivo `CHANGELOG.md` debe seguir la estructura de **Keep a Changelog**:

```markdown
# Changelog

Todos los cambios notables de este proyecto serán documentados en este archivo.

El formato está basado en [Keep a Changelog](https://keepachangelog.com/es-ES/1.0.0/),
y este proyecto sigue [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

### Changed

### Deprecated

### Removed

### Fixed

### Security
```

Reglas para el changelog:

- Mantener una sección `Unreleased` al principio.
- Al publicar una versión, mover los cambios de `Unreleased` a una sección con formato:

```markdown
## [1.2.3] - YYYY-MM-DD
```

- La versión más reciente debe aparecer primero.
- Agrupar los cambios por tipo.
- Usar las categorías:
  - `Added` para funcionalidades nuevas.
  - `Changed` para cambios en funcionalidades existentes.
  - `Deprecated` para funcionalidades obsoletas que se eliminarán más adelante.
  - `Removed` para funcionalidades eliminadas.
  - `Fixed` para corrección de errores.
  - `Security` para vulnerabilidades o cambios de seguridad.
- No usar el log de commits como sustituto del changelog.
- No registrar ruido irrelevante.
- Redactar entradas pensadas para humanos, no solo para máquinas.
- Cada entrada debe explicar el cambio de forma clara y concreta.
- Si un cambio rompe compatibilidad, indicarlo explícitamente.

Ejemplo de entrada:

```markdown
## [0.2.0] - 2026-05-05

### Added

- Añadida página de listado de Digimon con paginación.

### Changed

- Separado el cliente de Digi-API en `IDigiApiClient` y `DigiApiClient`.

### Fixed

- Corregido el estado vacío cuando la búsqueda no devuelve resultados.
```

### Flujo obligatorio por cambio

Cuando el asistente implemente o proponga cambios sobre el repositorio, debe comprobar esta lista:

1. Aplicar el cambio de código o configuración.
2. Actualizar o crear documentación relacionada en `docs/`.
3. Incrementar la versión cuando el cambio sea significativo.
4. Actualizar `CHANGELOG.md`.
5. Verificar que la aplicación compila, si tiene acceso al proyecto.
6. Resumir al usuario:
   - qué cambió
   - qué versión queda
   - qué documentación se actualizó
   - qué entrada se añadió al `CHANGELOG.md`

Si el asistente no puede actualizar alguno de estos puntos, debe indicarlo de forma explícita y explicar el motivo.

---

## Definition of Done

Una funcionalidad solo está terminada cuando:

- compila correctamente
- no rompe la navegación existente
- maneja estados de carga, vacío y error
- sigue la estructura de carpetas existente
- usa servicios y componentes existentes cuando sea apropiado
- no añade dependencias innecesarias
- mantiene la UI usable en escritorio y móvil
- tiene nombres claros y código legible

Para funcionalidades grandes, incluir también:

- tests básicos, si es práctico
- actualización del README si cambia la configuración o el comportamiento

---

## Comandos

Usar estos comandos como flujo local por defecto.

Restaurar:

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

Testear:

```bash
dotnet test
```

Formatear:

```bash
dotnet format
```

Si la estructura del proyecto cambia, actualizar estos comandos.

---

## Reglas de Git

Usar commits pequeños y con significado claro.

Estilo sugerido de commits:

```text
feat: add digimon list page
feat: add digimon detail page
fix: handle empty search results
refactor: separate api dto mapping
test: add digi api client tests
docs: update setup instructions
```

No mezclar cambios no relacionados en el mismo commit.

---

## Posición arquitectónica actual

La dirección preferida actual es:

```text
Blazor Web App
.NET 10
Interactive Server
Sin base de datos para el MVP
Digi-API consumida mediante un servicio tipado
DTOs separados de ViewModels
IMemoryCache para datos repetidos de la API
SQLite solo en una fase posterior
```

Esta decisión solo debe revisarse si cambian los requisitos del proyecto.
