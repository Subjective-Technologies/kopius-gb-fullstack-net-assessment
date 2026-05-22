# Stack & Dependencies

Locked choices for this assessment (no Docker):

| Layer | Choice | Project |
|-------|--------|---------|
| Runtime | **.NET 8** (LTS) | all |
| UI | **ASP.NET Core Razor Pages** | `GbIrl.Web` |
| Domain | Entities, OLV, statistics | `GbIrl.Core` |
| ORM / DB / Excel | **EF Core 8 + SQLite + ClosedXML** | `GbIrl.Infrastructure` |

All application code is **C#**.

---

## Solution layout

```
GbIrl.sln
global.json
samples/sample_irl.xlsx
src/
  GbIrl.Web/                  # UI — no NuGet beyond Web SDK
  GbIrl.Core/                 # domain — no external packages
  GbIrl.Infrastructure/       # EF Core, ClosedXML
Directory.Build.props         # shared net8.0, nullable, lock files
```

---

## Where dependencies live

| File | Role |
|------|------|
| [`src/GbIrl.Infrastructure/GbIrl.Infrastructure.csproj`](../src/GbIrl.Infrastructure/GbIrl.Infrastructure.csproj) | EF Core Sqlite, EF Design, ClosedXML |
| [`src/GbIrl.Infrastructure/packages.lock.json`](../src/GbIrl.Infrastructure/packages.lock.json) | Lock file for infrastructure packages |
| [`src/GbIrl.Web/GbIrl.Web.csproj`](../src/GbIrl.Web/GbIrl.Web.csproj) | Web SDK + project reference only |
| [`global.json`](../global.json) | .NET 8 SDK pin |

---

## NuGet packages (`GbIrl.Infrastructure`)

| Package | Version | Purpose |
|---------|---------|---------|
| `Microsoft.EntityFrameworkCore.Sqlite` | 8.0.11 | SQLite provider |
| `Microsoft.EntityFrameworkCore.Design` | 8.0.11 | Migrations (design-time) |
| `ClosedXML` | 0.104.2 | Parse `.xlsx` uploads |

---

## Commands

```bash
dotnet restore
dotnet build
dotnet run --project src/GbIrl.Web
```

```bash
dotnet ef migrations add <Name> \
  --project src/GbIrl.Infrastructure \
  --startup-project src/GbIrl.Web
```

---

## Configuration

`src/GbIrl.Web/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=gbirl.db"
}
```

SQLite DB file is created at runtime with EF Core `EnsureCreated()` for the assessment flow.

---

## Sample data

[`samples/sample_irl.xlsx`](../samples/sample_irl.xlsx)
