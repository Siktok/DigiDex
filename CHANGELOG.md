# Changelog

Todos los cambios notables de este proyecto seran documentados en este archivo.

El formato esta basado en [Keep a Changelog](https://keepachangelog.com/es-ES/1.0.0/),
y este proyecto sigue [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Anadidos filtros de Digimon por nivel, atributo y X-Antibody con opciones cargadas desde Digi-API.
- Anadida cobertura xUnit para metadata de filtros, paginacion por `nextPage`, cache y query strings de filtros.
- Anadida cobertura Playwright para controles de filtro y combinacion de busqueda, filtros y cambio a lista completa.
- Anadida suite E2E inicial con Playwright .NET para validar carga del listado, busqueda y navegacion al detalle usando Digi-API real.
- Anadido workflow de GitHub Actions para ejecutar los tests E2E de Playwright en pull requests y pushes a `main`.
- Anadida inicializacion SDD con OpenSpec, capacidades de testing detectadas y registro de skills en `.atl/skill-registry.md`.

### Changed

- Actualizada la documentacion principal y de versionado para reflejar los filtros de Digimon y la version `0.5.1`.
- Unificados los placeholders de filtros para mostrar `Todos` en nivel, atributo y X-Antibody.
- Version del proyecto incrementada a `0.5.0` por la nueva funcionalidad de filtros.
- Version del proyecto incrementada a `0.5.1` por el ajuste visual de placeholders de filtros.

### Deprecated

### Removed

### Fixed

### Security

## [0.2.2] - 2026-05-05

### Fixed

- Corregido el desbordamiento de imagenes en tarjetas, lista y detalle para evitar superposiciones con el contenido.

## [0.2.1] - 2026-05-05

### Changed

- Refinada la interfaz visual del listado y detalle para una experiencia mas actual.
- Alineados buscador y selector de vista dentro de una barra de controles consistente.
- Mejorada la jerarquia visual del banner, tarjetas, lista completa, estados y pagina de detalle.

## [0.2.0] - 2026-05-05

### Added

- Anadido selector de vista entre tarjetas paginadas y lista completa de Digimon.
- Anadido componente `DigimonFullList` para mostrar todos los Digimon en formato de lista.

### Changed

- Mejorado el aspecto visual del listado con cabecera destacada, tarjetas con mayor presencia y estados de resumen mas claros.
- Ampliado `IDigiApiClient` con carga completa paginada para construir la lista de todos los Digimon.

## [0.1.3] - 2026-05-05

### Fixed

- Corregido el binding del buscador para que no muestre `_searchTerm` como texto literal al entrar o volver al listado.

## [0.1.2] - 2026-05-05

### Fixed

- Anadido `.gitignore` para excluir archivos internos de Visual Studio y salidas de compilacion.
- Retirados del indice de Git los artefactos generados en `.vs`, `bin` y `obj` para evitar errores de permisos al confirmar cambios.

## [0.1.1] - 2026-05-05

### Changed

- Convertida la pagina de listado de Digimon en la pantalla principal de la aplicacion.
- Eliminada la barra lateral de navegacion y la pagina de inicio redundante.
- Cambiado el buscador para consultar dinamicamente mientras el usuario escribe.
- Ajustado el placeholder del buscador a `Escribe`.
- Aumentado el tamano visual del ID en las tarjetas de Digimon.

## [0.1.0] - 2026-05-05

### Added

- Creada estructura inicial del proyecto DigiDex con .NET 10 y Blazor Web App.
- Anadida integracion inicial con Digi-API mediante `IDigiApiClient`.
- Anadida pagina de listado de Digimon con paginacion.
- Anadida busqueda por nombre.
- Anadida pagina de detalle de Digimon.
- Anadidos componentes reutilizables para loading, error y empty state.
- Anadida documentacion inicial en `docs/`.
