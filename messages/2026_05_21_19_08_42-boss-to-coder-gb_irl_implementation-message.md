# boss to coder: Implement GB IRL Assessment (Phases 1–7)

## Context Check

I checked the latest thread message and the latest external logs before writing this.

Latest coder message read:
- *(none — this is the first message in thread `gb_irl_implementation`)*

Latest external logs at reply time:
- `dotnet test` baseline: **27 total, 8 passed, 19 failed** (OlvCalculator + TotalOlv + sample file only)

Protocol for this repo: [`agents_protocol.md`](agents_protocol.md). Read all `to-coder` and `to-all` messages before starting work.

---

## Mission

Implement the **Gordon Brothers IRL** assessment application so **all spec tests pass** (`dotnet test` → 27/27 green) and the app meets the business requirements. No Docker. No in-app AI/LLM for parsing or statistics.

**Definition of done:** `TASKS.md` Definition of Done checklist + full README (run instructions + Q&A).

---

## Authoritative documents (read in this order)

| Doc | Path | Use |
|-----|------|-----|
| Assessment spec | [`GB_FullStack_NET_Assessment_v4_en.md`](../GB_FullStack_NET_Assessment_v4_en.md) | Business rules, deliverables, Q&A |
| Task plan | [`TASKS.md`](../TASKS.md) | Phases 1–8, priority P0→P3 |
| Test contract | [`TESTS.md`](../TESTS.md) | Formulas, routes, test IDs S01–S37 |
| Stack | [`STACK.md`](../STACK.md) | Razor Pages, EF Core, SQLite, ClosedXML |
| Sample data | [`samples/sample_irl.xlsx`](../samples/sample_irl.xlsx) | ~31 rows for import testing |

---

## Solution layout (do not restructure)

```
src/GbIrl.Web/              → Razor Pages only (UI)
src/GbIrl.Core/             → Entities, OlvCalculator, IrlStatisticsService
src/GbIrl.Infrastructure/   → AppDbContext, ExcelImportService (ClosedXML)
tests/GbIrl.Spec.Tests/     → DO NOT weaken tests to pass; implement to satisfy them
```

**References:** `Web → Infrastructure → Core`

**Register in `Program.cs`:** `DbContext`, `ExcelImportService`, connection string from `appsettings.json` (`Data Source=gbirl.db`).

**EF migrations:**

```bash
dotnet ef migrations add InitialCreate --project src/GbIrl.Infrastructure --startup-project src/GbIrl.Web
dotnet ef database update --project src/GbIrl.Infrastructure --startup-project src/GbIrl.Web
```

Use in-memory or isolated SQLite in `GbIrlWebApplicationFactory` so web tests do not pollute dev DB.

---

## Implementation order (follow TASKS.md P0→P2)

### Phase 1 — Domain (partially done)

- `IrlItem`, `ItemCondition`, `OlvCalculator` exist — keep as single source of truth.
- Complete **`IrlStatisticsService`** per [`TESTS.md`](../TESTS.md):
  - `WeightedAverageUnitCost`: `Σ(unit_cost × quantity) / Σ(quantity)`; if `Σ(quantity)=0` → `0`.
  - `MedianUnitCost`: per-row `unit_cost`; even n → average of two middles.
  - `StandardDeviationUnitCost`: **sample** std dev (n−1); empty or n=1 → `0`.
- **Tests to turn green:** S11–S16.

### Phase 2 — Persistence

- Wire `AppDbContext` with `DbSet<IrlItem>`.
- Apply migrations on startup or document `dotnet ef database update` in README.
- Service/repository: list all, get by id, update, filter by `category` and `condition` query params.
- **No auth.**

### Phase 3 — Excel import

- Implement `ExcelImportService.Import(Stream)` with **ClosedXML**.
- Map columns: SKU, Description, Category, Quantity, Unit Cost, Condition (sheet `IRL`, header row 1).
- Validate rows; reject with `ImportRowError` (row number + reason).
- **Duplicate SKU policy:** reject duplicate with reason (document in README).
- Razor Page **`/Upload`**: multipart POST, persist valid rows, show loaded/rejected summary.
- **Tests to turn green:** S21–S25.

### Phase 4 — Items list

- Razor Page **`/Items`**: table with SKU, Description, Category, Quantity, Unit Cost, Condition, **OLV** (computed via `OlvCalculator`, not stored unless you document choice in README).
- Show **Total OLV** for current row set.
- Filters: `?category=` and `?condition=` — table and aggregates use **filtered subset only**.
- **Tests to turn green:** S30–S35, S36 (partial).

### Phase 5 — Statistics on page

- Display on `/Items`: Total OLV, weighted avg, median, std dev (labels must match test regexes in `IrlItemsPageTests.cs`: "Total OLV", "Weighted", "Median", "Standard Deviation" or "Std. Dev").
- Recompute when filters change.
- **Tests:** S35, S36.

### Phase 6 — Edit item

- **`/Items/Edit/{id}`**: edit **quantity**, **unit cost**, **condition** only; save → persist → OLV recalculates.
- After edit, list shows updated OLV (e.g. id=1 edited to qty=99, cost=10, Damaged → OLV **198** per S37).
- **Tests to turn green:** S37.

### Phase 7 — README

Update [`README.md`](../README.md) with:

1. Step-by-step run on Mac/Linux (`dotnet restore`, `build`, `ef database update`, `run`, URL).
2. Stack justification (point to STACK.md).
3. Shortcuts / honesty section.
4. Q&A answers (Excel lib, stats formulas, hardest decision, left out, unsure code).

---

## Routes contract (tests enforce these)

| Route | Method | Purpose |
|-------|--------|---------|
| `/Upload` | GET | Upload form |
| `/Upload` | POST | `file` field, `.xlsx` |
| `/Items` | GET | Table + stats; optional `category`, `condition` |
| `/Items/Edit/{id}` | GET | Edit form |
| `/Items/Edit/{id}` | POST | `Quantity`, `UnitCost`, `Condition` |

See `tests/GbIrl.Spec.Tests/Support/AppRoutes.cs`.

---

## Constraints

1. **No LLM/OpenAI/Anthropic** in runtime for Excel or stats.
2. **No Docker** required.
3. **Do not delete or skip** failing tests — implement until green.
4. **C# only** for app logic; Razor for UI.
5. Developed on Linux; README must work for Mac reviewer (`dotnet run`).

---

## Verification (required in your reply message)

```bash
dotnet build
dotnet test
dotnet run --project src/GbIrl.Web
```

Paste full `dotnet test` summary in your response to boss.

Manual: upload `samples/sample_irl.xlsx`, verify table, filters, edit one row.

---

## What I Did Not Change

- No application feature code — only this message and protocol copy.
- Existing stubs: `OlvCalculator`, `TotalOlv`, empty `ExcelImportService.Import` throwing `NotImplementedException`.

---

## Next Step

**coder:** Read this message and [`agents_protocol.md`](agents_protocol.md). Implement Phases 1–7 in priority order. When done (or blocked), write:

`YYYY_MM_DD_HH_MM_SS-coder-to-boss-gb_irl_implementation-message.md`

Include: files modified, `dotnet test` output, what you did not change, and remaining gaps if any.

**boss** will review test results and either accept or send follow-up tasks.
