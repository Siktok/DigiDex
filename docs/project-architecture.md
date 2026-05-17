# Arquitectura del proyecto

## Objetivo

DigiDex es un MVP de aprendizaje para consultar Digimon desde Digi-API con .NET 10 y Blazor Web App. La prioridad es una base clara, mantenible y facil de extender sin introducir capas innecesarias.

## Archivos afectados

- `src/DigiDex.Web/Program.cs`
- `src/DigiDex.Web/Components/`
- `src/DigiDex.Web/Models/`
- `src/DigiDex.Web/Services/`
- `src/DigiDex.Web/Options/`

## Decision tomada

La aplicacion usa Blazor Web App con renderizado Interactive Server. Los componentes Razor son responsables de la UI y sus estados, mientras que el acceso HTTP queda encapsulado en `IDigiApiClient` y `DigiApiClient`.

La pantalla principal es el listado de Digimon, disponible en `/` y `/digimon`. No se mantiene una pagina de inicio separada ni una barra lateral de navegacion porque el MVP solo tiene un flujo principal.

El listado ofrece dos modos de presentacion:

- Tarjetas paginadas para exploracion visual.
- Lista completa para revisar todos los Digimon en una vista compacta.

La estructura separa:

- `Components/Pages`: paginas con rutas. El listado de Digimon es la entrada principal.
- `Components/Digimon`: componentes reutilizables de dominio, incluyendo tarjetas, busqueda, evoluciones y lista completa.
- `Components/Shared`: estados comunes de carga, error y vacio.
- `Models/Api`: DTOs tolerantes con la forma de Digi-API.
- `Models/ViewModels`: modelos seguros para renderizar en UI.
- `Services`: integracion con Digi-API.
- `Options`: configuracion fuertemente tipada.
- `tests/DigiDex.Web.E2ETests`: pruebas E2E con Playwright .NET para validar flujos reales en navegador.

## Motivo

Interactive Server permite construir el MVP con interactividad, menor complejidad de hosting y sin mover la aplicacion a WebAssembly. Separar DTOs y ViewModels evita acoplar la UI a cambios menores de la API externa.

## Alternativas descartadas

- Blazor WebAssembly: innecesario para el MVP inicial.
- EF Core o base de datos: no hay persistencia requerida.
- Clean Architecture con muchas capas: aumentaria complejidad sin resolver un problema actual.
- MediatR o AutoMapper: no aportan suficiente valor en esta escala.

## Impacto

La aplicacion queda compilable, con un flujo directo hacia el listado y preparada para agregar filtros, favoritos o tests sin rehacer la estructura.

La suite E2E comprueba el flujo principal desde fuera de la aplicacion, como lo haria un usuario: abrir el listado, buscar y navegar al detalle. Estos tests no sustituyen futuros tests unitarios del cliente API o mapeos, pero cubren regresiones visibles del MVP.

## Mantenimiento

Agregar nuevas paginas en `Components/Pages`, componentes reutilizables en `Components/Digimon` o `Components/Shared`, y nueva logica de API en `IDigiApiClient` cuando exista una necesidad de UI real.

Mantener los tests Playwright centrados en flujos criticos y selectores accesibles. Si la suite crece, separar tests estables con datos controlados de pruebas opcionales contra Digi-API real.
