# Spec-Driven Development

## Objetivo

Inicializar DigiDex para trabajar con Spec-Driven Development usando OpenSpec como almacén de artefactos.

## Archivos afectados

- `openspec/config.yaml`
- `openspec/specs/`
- `openspec/changes/`
- `.atl/skill-registry.md`

## Decisión tomada

Se usa `artifact_store=openspec`. El contexto del proyecto, las reglas de fases SDD y las capacidades de testing quedan versionadas en `openspec/config.yaml`.

## Motivo

OpenSpec deja los artefactos de especificación dentro del repositorio, lo que facilita revisión, continuidad entre sesiones y trazabilidad sin depender de memoria local como backend principal.

## Alternativas descartadas

- Engram como backend principal: útil para memoria persistente, pero no es el modo solicitado para esta inicialización.
- Sin persistencia: no cumpliría la necesidad de conservar contexto SDD entre fases.

## Impacto sobre el proyecto

- Las próximas fases SDD deben crear cambios bajo `openspec/changes/{change-name}/`.
- El modo Strict TDD queda activado porque existe runner de tests (`dotnet test`) y no había marcador explícito previo.
- Las capacidades detectadas son: E2E con xUnit + Playwright, tests unitarios con xUnit en `tests/DigiDex.Web.Tests`, type-check vía `dotnet build` y formato vía `dotnet format`; no se detectó cobertura.

## Mantenimiento

Actualizar `openspec/config.yaml` cuando cambien el stack, comandos de test, reglas de fase o capacidades reales de testing.
