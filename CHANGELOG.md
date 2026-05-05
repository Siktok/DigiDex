# Changelog

Todos los cambios notables de este proyecto seran documentados en este archivo.

El formato esta basado en [Keep a Changelog](https://keepachangelog.com/es-ES/1.0.0/),
y este proyecto sigue [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

### Changed

### Deprecated

### Removed

### Fixed

### Security

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
