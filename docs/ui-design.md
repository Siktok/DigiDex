# Diseno de interfaz

## Objetivo

Dar a DigiDex una interfaz mas actual sin anadir dependencias ni JavaScript extra. El diseno debe seguir siendo simple, responsive y facil de mantener.

## Archivos afectados

- `src/DigiDex.Web/wwwroot/app.css`
- `src/DigiDex.Web/Components/Pages/DigimonList.razor`
- `src/DigiDex.Web/Components/Pages/DigimonDetails.razor`
- `src/DigiDex.Web/Components/Digimon/DigimonSearchBox.razor`

## Decision tomada

Se usa un sistema visual compacto con:

- Banner principal oscuro con metricas claras.
- Barra de controles con buscador y selector de vista alineados.
- Tarjetas con imagen destacada, sombra moderada y acento amarillo.
- Lista completa en filas densas y escaneables.
- Detalle con panel principal, imagen destacada y metadatos en bloques.

## Motivo

La aplicacion solo tiene un flujo principal, por lo que la UI debe invitar a explorar sin convertirse en una landing page. La prioridad es que el listado, la busqueda y el cambio de vista se entiendan de inmediato.

## Alternativas descartadas

- Libreria de componentes: innecesaria para el MVP.
- JavaScript para microinteracciones: no aporta suficiente valor por ahora.
- Diseno muy decorativo: puede dificultar la lectura repetida del listado.

## Mantenimiento

Mantener radios de `8px`, controles con altura similar y estados visuales reutilizables. Si se anaden filtros, deben integrarse en la misma barra de controles o en un bloque alineado con ella.

Las imagenes externas de Digi-API deben renderizarse siempre dentro de contenedores con `overflow: hidden`, `box-sizing: border-box` y `object-fit: contain` para evitar que ilustraciones con lienzos grandes se superpongan al texto.
