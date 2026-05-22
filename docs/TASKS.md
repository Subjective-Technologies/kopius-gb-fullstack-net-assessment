# Gordon Brothers IRL Assessment — Task Plan

**Scope:** ~3 hours implementation · **Delivery:** 24–48h · **Constraint:** No LLM/AI inside the app for parsing or stats.

**Rule for implementer:** Do not use AI services in runtime logic. Calculations and Excel parsing must be code/standard libraries. Using Cursor/Copilot to *write* code is expected.

---

## Phase 0 — Prerequisites & Repo Setup

**Stack prep (done):** See [`STACK.md`](STACK.md) — `GbIrl.sln`, `src/GbIrl.Web/`, NuGet packages in `.csproj`, `global.json`, `packages.lock.json`. No Docker.

**Spec tests (done):** See [`TESTS.md`](TESTS.md) — `tests/GbIrl.Spec.Tests`, run `dotnet test`. Red until phases 1–6 implemented.

| ID | Task | Done when |
|----|------|-----------|
| 0.1 | Obtain `sample_irl.xlsx` (~30 rows: SKU, description, category, quantity, unit cost, condition) and place in repo (e.g. `/samples/` or document upload-only flow). | ✅ `samples/sample_irl.xlsx` |
| 0.2 | Initialize .NET 8+ (or latest LTS) solution: Web app project + optional class library for domain/services. | ✅ `dotnet build` succeeds |
| 0.3 | Choose and document stack in README (ORM, DB, Excel lib, UI: Razor/Blazor/MVC, auth if any). | README § Stack with 1–2 sentence justification per choice. |
| 0.4 | Choose run strategy for **reviewer Mac**: `dotnet run` **or** `docker compose up` **or** hybrid; document every step (migrate, seed, env vars). | README § How to run — copy-pasteable, nothing omitted. |
| 0.5 | Git repo clean: `.gitignore` for `bin/`, `obj/`, user secrets, local DB files if applicable. | Repo ready to zip or push. |

---

## Phase 1 — Domain Model & Business Rules

| ID | Task | Done when |
|----|------|-----------|
| 1.1 | Define **Item** entity/DTO: SKU, Description, Category, Quantity, UnitCost, Condition (enum or constrained string). | Model matches Excel columns. |
| 1.2 | Define **Condition** factors: `New = 0.70`, `Used = 0.50`, `Damaged = 0.20`. | Single source of truth (constant map or enum extension). |
| 1.3 | Implement **OLV** calculation: `OLV = unit_cost × quantity × factor_by_condition`. | Unit tests or manual check on 3 conditions. |
| 1.4 | Decide persistence: store computed OLV on row **or** compute on read; document tradeoff in README if non-obvious. | Edit/save always shows correct OLV. |

---

## Phase 2 — Persistence

| ID | Task | Done when |
|----|------|-----------|
| 2.1 | Configure database (SQLite for simplicity on Mac is fine; SQL Server/Postgres OK if Docker documents it). | Connection string in `appsettings` or env. |
| 2.2 | EF Core (or chosen ORM) migrations for Item table (+ import batch metadata if tracking upload summary). | `dotnet ef database update` (or auto-migrate on startup) documented in README. |
| 2.3 | Repository/service: CRUD for items, list all, filter by category and condition. | Queries return filtered subsets for stats. |

---

## Phase 3 — Excel Import (Upload Screen)

| ID | Task | Done when |
|----|------|-----------|
| 3.1 | Add **upload screen** (multipart form): accept `.xlsx`. | UI reachable from home/nav. |
| 3.2 | Parse Excel with chosen library (e.g. ClosedXML, EPPlus, ExcelDataReader — **answer in README Q1**). | Rows mapped to Item fields. |
| 3.3 | **Validation per row**: required fields, numeric quantity/unit cost, valid condition, duplicate SKU policy (reject or update — document choice). | Invalid rows not persisted. |
| 3.4 | **Persist** valid rows. | DB contains imported items after upload. |
| 3.5 | **Post-upload summary**: counts **loaded** vs **rejected**, with **reason per rejection** (row number + message). | User sees summary on same page or redirect. |

**Acceptance:** Upload `sample_irl.xlsx` → summary shows ~30 loaded (minus any intentional bad rows if you add test data).

---

## Phase 4 — Item List & Table UI

| ID | Task | Done when |
|----|------|-----------|
| 4.1 | **Table** listing all items: SKU, Description, Category, Quantity, Unit Cost, Condition, **OLV** (calculated per row). | All columns visible. |
| 4.2 | Display **Total OLV** (sum of OLV for displayed rows). | Matches manual sum on sample data. |
| 4.3 | **Filters**: Category dropdown/checkbox, Condition dropdown/checkbox. | Table updates to subset. |
| 4.4 | When filters applied, stats (4.2 and Phase 5) use **visible subset only**. | Document in README if labels say "filtered". |

---

## Phase 5 — Statistics Panel (Subset-Aware)

Implement in a dedicated service; formulas documented in README Q2.

| ID | Stat | Formula / decision |
|----|------|-------------------|
| 5.1 | **Weighted average unit cost** | `Σ(unit_cost × quantity) / Σ(quantity)` over current rows. Edge case: `Σ(quantity) = 0` → show N/A or 0; document. |
| 5.2 | **Median unit cost** | Sort `unit_cost` values (clarify: one entry per **row/item**, not repeated by quantity — **document choice in README**). Even n: average of two middles; odd n: middle value. |
| 5.3 | **Standard deviation of unit cost** | Population vs sample: pick one (e.g. **sample** `n-1` if treating rows as sample; **population** `n` if full inventory — **state in README**). Formula: `sqrt(Σ(x - μ)² / divisor)`. |
| 5.4 | Wire stats to **filtered** item set same as Total OLV. | Change filter → all four metrics update. |

---

## Phase 6 — Edit Item

| ID | Task | Done when |
|----|------|-----------|
| 6.1 | Edit UI: allow changing **quantity**, **unit cost**, **condition** only (SKU/description/category read-only unless you justify otherwise in README). | Form validation (positive quantity, non-negative cost). |
| 6.2 | On save: persist changes and **recalculate OLV**. | Table and totals refresh. |
| 6.3 | Total OLV and stats 5.1–5.3 update after edit without full page confusion (HTMX/Blazor/post-redirect acceptable). | Reviewer can demo live edit in interview. |

---

## Phase 7 — README & Q&A (Required Deliverable)

| ID | Section | Content |
|----|---------|---------|
| 7.1 | How to run | Step-by-step on Mac: prerequisites (.NET SDK version), clone, restore, migrate, run command(s), URL, default credentials if any. |
| 7.2 | Stack & why | ORM, DB, Excel lib, UI framework — brief justification each. |
| 7.3 | Shortcuts / honesty | Hardcoded paths, seeded user, skipped auth, no migrations — **list explicitly**. |
| 7.4 | What you left out & why | If incomplete; prioritized backlog. |
| 7.5 | **Q&A answers** (brief): | |
| | Q1 | Which Excel library and why? |
| | Q2 | Statistical approach: formulas, median weighting choice, std dev population vs sample. |
| | Q3 | Hardest decision during exercise. |
| | Q4 | What you left out; if +3 hours, what first? |
| | Q5 | Code you're unsure about. |

---

## Phase 8 — Smoke Test Before Submit

| ID | Check |
|----|-------|
| 8.1 | Fresh clone on Mac (or clean machine): follow README only → app starts. |
| 8.2 | Upload `sample_irl.xlsx` → summary → table populated. |
| 8.3 | Total OLV matches spreadsheet hand-check for 2–3 rows. |
| 8.4 | Filter by one category → totals and stats change. |
| 8.5 | Edit one item quantity → OLV and aggregates update. |
| 8.6 | No AI API keys required to run. |

---

## Priority Order (If Time Runs Out)

Implement in this order; stop at first cutoff and document remainder in README §7.4:

1. **P0 — Must run:** 0.x, 1.x, 2.x minimal, 3.x upload + persist + summary, 4.1 table with OLV, 7.1 run instructions.
2. **P1 — Core value:** 4.2 total OLV, 6.x edit + recalc, 5.1 weighted average.
3. **P2 — Full spec:** 5.2 median, 5.3 std dev, 4.3–4.4 filters driving stats.
4. **P3 — Polish:** Docker, auth, integration tests, pretty UI.

---

## Solution Structure (implemented)

```
/
├── GbIrl.sln
├── README.md
├── samples/sample_irl.xlsx
└── src/
    ├── GbIrl.Web/              # UI + Razor Pages
    ├── GbIrl.Core/             # Entities, OLV, statistics
    └── GbIrl.Infrastructure/   # EF Core, Excel import
```

See [README.md](../README.md) for references and EF commands.

---

## Out of Scope (Unless Extra Time)

- Authentication / multi-user IRLs
- Version history per item
- Export back to Excel
- LLM-based parsing or analytics
- Production hardening (CI, cloud deploy)

---

## Definition of Done

- [ ] App runs on Mac per README
- [ ] Excel upload with loaded/rejected summary
- [ ] Item table with OLV column
- [ ] Edit quantity, unit cost, condition → OLV recalculates
- [ ] Total OVL + weighted avg + median + std dev (on full set and filtered subset)
- [ ] README complete including Q&A and honest gaps
- [ ] No in-app AI for parsing or statistics

---

*This file is planning only. Implementation is tracked separately; do not treat TASKS.md as a substitute for README deliverable requirements.*
