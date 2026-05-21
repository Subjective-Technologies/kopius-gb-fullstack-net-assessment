# Senior Full-Stack .NET Engineer — Technical Assessment

## Contexto del negocio (leer primero)

**Gordon Brothers (GB)** es una firma de appraisal y liquidación de activos. Cuando una empresa entra en distress, GB la visita, releva su inventario, y le asigna un valor de liquidación a cada item para que un banco pueda usar ese inventario como colateral.

El listado de items relevados se llama **IRL** (*Item Record List*). Es una planilla donde cada fila es un item con sus atributos básicos: SKU, descripción, categoría, cantidad, costo unitario, condición.

El valor que se le asigna a cada item se llama **OLV** (*Orderly Liquidation Value*) — cuánto vale ese item si se vende en una liquidación ordenada. Se calcula con una fórmula simple:

```
OLV = unit_cost * quantity * factor_by_condition
```

donde `factor_by_condition`: `New = 0.70`, `Used = 0.50`, `Damaged = 0.20`.

---

## Scope

- **Tiempo estimado de trabajo:** ~3 horas
- **Entrega:** 24–48 horas desde recibido
- **Qué evaluamos:** que puedas levantar un proyecto .NET desde cero, decidir el stack auxiliar, integrar varias cosas a la vez, y entregar algo que corra. Si no llegás a todo, decilo en el README — preferimos honestidad a humo.

---

## Stack

- **Obligatorio:** .NET (8 o LTS más reciente)
- **El resto lo elegís vos:** ORM, base de datos, librería para parsear Excel, framework de UI (Razor, Blazor, MVC, lo que sea), auth, lo que necesites.

Justificá brevemente en el README qué elegiste y por qué.

**Importante:** no se permite usar LLMs/OpenAI/Anthropic *dentro* de la app (ej. parsear el Excel con AI, calcular stats con AI). Esos cálculos los hacés con código o con librerías standard. Para *escribir el código* sí podés (y se espera que) uses Copilot / Cursor / Claude / lo que tengas — queremos ver vibecoding bien hecho.

---

## Cómo lo corremos

Tu README tiene que decirnos **cómo correrlo en la máquina del reviewer**. Las opciones razonables:

- `dotnet run` y listo (lo más simple)
- `docker compose up` (si te conviene aislar dependencias)
- Una combinación con instrucciones claras (build, migrate, seed, run)

No nos importa cuál elijas. Nos importa que esté documentado y que efectivamente arranque siguiendo los pasos del README. Si tuviste que hacer algún atajo (ej. hardcodear el path del Excel, dejar un usuario en código, saltearte migrations) → contalo en el README. Eso suma, no resta.

---

## Funcionalidad a entregar

Te damos un Excel de muestra (`sample_irl.xlsx`, ~30 items con SKU, descripción, categoría, cantidad, costo unitario, condición).

La app tiene que:

1. **Importar** el Excel desde una pantalla de upload. Persistir los items. Mostrar summary post-upload (cargados / rechazados con motivo).
2. **Listar** los items en una tabla con SKU, descripción, categoría, cantidad, costo unitario, condición, y **OLV calculado** por fila.
3. **Editar** un item (cantidad, costo unitario, condición). Al guardar, el OLV se recalcula.
4. **Mostrar el OLV total** del IRL (suma de OLV de todas las filas).
5. **Mostrar el promedio ponderado del costo unitario**, ponderado por cantidad. (Si no sabés qué es un promedio ponderado: investigá — es parte del ejercicio.)
6. **Mostrar la mediana del costo unitario** del inventario.
7. **Mostrar el desvío estándar (standard deviation) del costo unitario** del inventario.
8. **Filtrar la tabla por categoría y por condición**. Cuando se aplica un filtro, los stats 4-7 se recalculan sobre el subset visible.

Si no llegás a todo, priorizá. En el README contá qué dejaste afuera y por qué.

---

## Entregable

- Git repo (preferido) o zip
- README con:
  - Cómo correrlo (paso a paso, sin omitir nada)
  - Stack que elegiste y por qué
  - Qué dejaste afuera y por qué
  - Respuestas a la Q&A de abajo

---

## Q&A (responder en el README, breve)

1. ¿Qué librería usaste para parsear el Excel? ¿Por qué esa?
2. Contanos cómo abordaste los **cálculos estadísticos** (promedio ponderado, mediana, desvío estándar): qué fórmulas aplicaste y qué decisiones tomaste.
3. ¿Qué decisión te costó más durante el ejercicio?
4. ¿Qué dejaste afuera? Si tuvieras 3 horas más, ¿qué agregarías primero?
5. ¿Qué parte del código te genera dudas o no te termina de cerrar?

---

## Notas finales

- No buscamos código perfecto. Buscamos código honesto.
- Si algo te frustró o no lo resolviste bien, decilo. Suma más que disimularlo.
- Vamos a correr el proyecto en vivo durante la entrevista. Estate preparado para explicar cualquier línea.
