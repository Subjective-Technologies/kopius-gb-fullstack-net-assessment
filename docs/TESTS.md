# Spec Test Contract

Automated tests in `tests/GbIrl.Spec.Tests` verify [GB_FullStack_NET_Assessment_v4_en.md](../GB_FullStack_NET_Assessment_v4_en.md).

## Run

```bash
dotnet test
```

## Statistical conventions (locked by tests)

| Metric | Formula |
|--------|---------|
| **OLV** | `unit_cost × quantity × factor` (New 0.70, Used 0.50, Damaged 0.20) |
| **Total OLV** | Sum of per-row OLV |
| **Weighted avg unit cost** | `Σ(unit_cost × quantity) / Σ(quantity)` |
| **Median unit cost** | Median of **per-row** `unit_cost` (one value per item, not expanded by quantity). Even count: average of two middle values. |
| **Std dev unit cost** | **Sample** standard deviation (divisor `n − 1`). Empty or single row → `0`. |

Filtered table: stats 4–7 recomputed on the **visible subset** only (same formulas).

## Test map

| ID | Area | Requirement |
|----|------|-------------|
| S01–S06 | Core | OLV factors and formula |
| S10–S16 | Core | Total OLV, weighted avg, median, std dev |
| S20–S25 | Infrastructure | Excel import valid/invalid rows |
| S30–S37 | Web | Upload, list, edit, filters, subset stats |

## Expected routes (Web)

| Route | Purpose |
|-------|---------|
| `/Upload` | POST multipart `.xlsx`, summary loaded/rejected |
| `/Items` | Table + aggregates; query `category`, `condition` |
| `/Items/Edit/{id}` | POST quantity, unit cost, condition |

## Status

Run: `dotnet test`

| When | Result |
|------|--------|
| Pre-implementation (boss baseline) | 8 passed, 19 failed |
| Post-implementation (coder, verified by boss) | **27 passed, 0 failed** |

Target met: full assessment spec test contract is green.
