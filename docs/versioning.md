# Versionado

## Objetivo

Mantener trazabilidad de los cambios significativos del proyecto con Semantic Versioning y `CHANGELOG.md`.

## Decision tomada

La version central vive en `Directory.Build.props`:

```xml
<Version>0.2.2</Version>
<AssemblyVersion>0.2.2.0</AssemblyVersion>
<FileVersion>0.2.2.0</FileVersion>
```

## Semantic Versioning

- `MAJOR`: cambios incompatibles o redisenos que rompan comportamiento.
- `MINOR`: nuevas funcionalidades compatibles hacia atras.
- `PATCH`: correcciones, ajustes internos o refactors sin cambio funcional visible.

## CHANGELOG.md

El changelog sigue Keep a Changelog. Mantener siempre `Unreleased` al inicio y mover entradas a una version fechada cuando se publique.

## Flujo de mantenimiento

1. Aplicar el cambio.
2. Actualizar documentacion en `docs/` si afecta comportamiento, arquitectura o configuracion.
3. Incrementar la version en `Directory.Build.props` si el cambio es significativo.
4. Registrar el cambio en `CHANGELOG.md`.
5. Compilar con `dotnet build`.

## Alternativas descartadas

Duplicar la version en el `.csproj` o en `appsettings.json` se descarta por ahora para evitar inconsistencias. Solo deberia hacerse si la version necesita mostrarse en runtime.

## Archivos fuera de Git

El repositorio usa `.gitignore` para excluir archivos generados o locales:

- `.vs/`: indices, cache y estado interno de Visual Studio.
- `bin/` y `obj/`: resultados de compilacion y archivos intermedios.
- `TestResults/`, paquetes NuGet generados y logs.

Estos archivos pueden estar bloqueados por Visual Studio o por `dotnet run`, por lo que no deben formar parte de los commits.
