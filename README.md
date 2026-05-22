# Gordon Brothers — IRL Assessment

Item Record List (IRL) app: import Excel, persist items, calculate OLV and inventory statistics.

**Status:** Assessment implementation complete for the spec-test contract in [docs/TESTS.md](docs/TESTS.md).

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (`dotnet --version` → 8.0.x)
- Optional: `dotnet tool install --global dotnet-ef` (migrations)

## Run (no Docker)

**macOS / Linux (recommended):**

```bash
cd kopius-gb-fullstack-net-assessment   # repo root
./run.sh
```

Or manually:

```bash
cd kopius-gb-fullstack-net-assessment   # repo root
dotnet restore
dotnet build -m:1
dotnet run --project src/GbIrl.Web
```

`./run.sh` opens the browser on macOS and listens on `http://127.0.0.1:5136` (override with `PORT=5249 ./run.sh`). Set `OPEN_BROWSER=0` to skip `open`.

Open the URL shown in the console, then use `/Upload` to upload `samples/sample_irl.xlsx`.

The app uses SQLite at `src/GbIrl.Web/gbirl.db` by default and creates the database schema automatically on startup with EF Core `EnsureCreated()`. No Docker, external database, API key, or migration command is required to run the reviewer flow.

## Tests

```bash
dotnet test
```

See [docs/TESTS.md](docs/TESTS.md) for formulas, routes, and test IDs (S01-S37).

## Project layout

Matches [docs/TASKS.md](docs/TASKS.md) suggested structure:

```
/
├── GbIrl.sln
├── global.json
├── README.md                 ← run instructions + Q&A (assessment deliverable)
├── docs/
│   ├── STACK.md              ← stack and NuGet dependencies
│   ├── TASKS.md              ← implementation checklist
│   └── TESTS.md              ← executable spec-test contract
├── samples/
│   └── sample_irl.xlsx
├── tests/
│   └── GbIrl.Spec.Tests/     # Spec tests (see TESTS.md)
└── src/
    ├── GbIrl.Web/            # Razor Pages UI
    │   ├── Pages/
    │   ├── wwwroot/
    │   └── Program.cs
    ├── GbIrl.Core/           # Domain: entities, OLV, statistics
    │   ├── Entities/
    │   ├── Enums/
    │   └── Services/
    └── GbIrl.Infrastructure/ # EF Core SQLite, Excel import
        ├── Data/
        └── Excel/
```

### Project references

```
GbIrl.Web → GbIrl.Infrastructure → GbIrl.Core
```

### EF migrations

This build uses `EnsureCreated()` for the small assessment database so `dotnet run` is enough. If this were going beyond the assessment, the next step would be replacing that with migrations:

```bash
dotnet ef migrations add InitialCreate \
  --project src/GbIrl.Infrastructure \
  --startup-project src/GbIrl.Web
dotnet ef database update \
  --project src/GbIrl.Infrastructure \
  --startup-project src/GbIrl.Web
```

## Stack

See [docs/STACK.md](docs/STACK.md): Razor Pages, EF Core, SQLite, ClosedXML, .NET 8.

- Razor Pages keeps the UI straightforward for upload, table filtering, and edit forms without a front-end build chain.
- EF Core with SQLite gives local persistence that runs on Mac/Linux with only the .NET SDK.
- ClosedXML reads `.xlsx` files directly and has a small, understandable API for worksheet/header-based imports.

## Behavior Notes

- Upload policy: a new upload replaces the current IRL. Within a file, duplicate SKUs are rejected and shown in the import summary.
- OLV is computed when displayed rather than stored, so edits always use the current formula.
- Filters on `/Items?category=&condition=` apply to the table and all aggregate statistics.
- No authentication, multi-user isolation, export, Docker, or AI/LLM runtime parsing/statistics were added.

## Q&A

**Which library did you use to parse the Excel file? Why that one?**  
ClosedXML. It supports `.xlsx` directly, reads worksheets and cells without Excel installed, and is simple enough for explicit validation logic.

**How did you approach the statistical calculations?**  
Total OLV is the sum of `unit_cost * quantity * condition_factor`. Weighted average is `sum(unit_cost * quantity) / sum(quantity)`, returning `0` when quantity sums to zero. Median uses one unit-cost value per row, not quantity-expanded values. Standard deviation follows the executable spec tests and uses the sample divisor `n - 1`, returning `0` for empty or single-row sets.

**Which decision was the hardest?**  
The import replacement policy. I chose replacing the current IRL on upload because the assessment describes importing one IRL at a time and it keeps repeated demo/test uploads deterministic.

**What did you leave out? If you had 3 more hours, what would you add first?**  
I left out authentication, import batch history, Excel export, richer UI styling, and production migrations. With more time, I would add migrations plus import-batch records so prior uploads and rejection summaries can be audited.

**Which part of the code makes you unsure or does not fully satisfy you?**  
Using `EnsureCreated()` is intentionally pragmatic for the assessment, but migrations are the better long-term path once the schema needs controlled evolution.

## Assessment docs

- [GB_FullStack_NET_Assessment_v4_en.md](GB_FullStack_NET_Assessment_v4_en.md)
