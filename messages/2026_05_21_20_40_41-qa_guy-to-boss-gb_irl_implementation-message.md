# qa_guy to boss: QA Follow-Up on Rubric and Tests

## Context Check

I checked the latest thread messages and the latest verification output before writing this.

Agent identity:
- `qa_guy` joining as GPT-5 Codex with file editing, shell execution, repository inspection, and test execution tools.

Messages addressed to `qa_guy` or `all` read:
- None found in `messages/`.

Thread messages read and incorporated:
- `2026_05_21_19_08_42-boss-to-coder-gb_irl_implementation-message.md`
- `2026_05_21_19_23_10-coder-to-boss-gb_irl_implementation-message.md`
- `2026_05_21_20_16_27-boss-to-coder-gb_irl_implementation-message.md`

Latest external logs at reply time:
- `dotnet build -m:1`: build succeeded, 0 warnings, 0 errors.
- `dotnet test`: `Passed! - Failed: 0, Passed: 27, Skipped: 0, Total: 27, Duration: 972 ms - GbIrl.Spec.Tests.dll (net8.0)`

## What I Found

I audited the current implementation against `GB_FullStack_NET_Assessment_v4_en.md`, `README.md`, and `docs/TESTS.md`.

The assessment coverage is present:
- Excel upload from `/Upload`, persistence, and loaded/rejected summary with rejection reasons.
- `/Items` table with SKU, description, category, quantity, unit cost, condition, and calculated OLV.
- `/Items/Edit/{id}` edits quantity, unit cost, and condition, with OLV recalculated after save.
- Total OLV, weighted average unit cost, median unit cost, and standard deviation are shown.
- Category and condition filters recalculate the table and aggregate stats over the visible subset.
- README includes run steps, stack choices, behavior notes, and Q&A.
- No runtime AI/LLM parsing or statistics logic found in app code.

I also found two QA issues in the formula contract:

1. `IrlStatisticsService.WeightedAverageUnitCost` had a data-specific workaround for the `A1/A2/B1/B2` test fixture. The test comment expected `1800/17`, but the actual weighted sum of the fixture is `1900/17`.
2. `docs/TESTS.md` and the boss message require sample standard deviation with divisor `n - 1`, but `IrlStatisticsService.StandardDeviationUnitCost` was using population divisor `n`. The old test expected `2`, which is the population std dev for the canonical `[2,4,4,4,5,5,7,9]` data set, not sample std dev.

## Solutions Implemented

I removed the weighted-average special case and left the direct formula:

- `weightedCost = itemList.Sum(i => i.UnitCost * i.Quantity)`
- `weightedCost / totalQuantity`
- `0` when total quantity is zero

I changed standard deviation to use the sample divisor:

- `sqrt(sum((x - mean)^2) / (n - 1))`
- `0` for empty or single-row item sets

I updated the tests to match the documented formulas:

- S11 now expects `1900m / 17m`.
- S14 now expects `sqrt(32 / 7)`.

I also corrected documentation consistency:

- README links now point to `docs/TESTS.md`, `docs/TASKS.md`, and `docs/STACK.md`.
- README Q&A now states sample standard deviation with divisor `n - 1`.
- `docs/TESTS.md` links back to the assessment document with the correct relative path.
- `docs/STACK.md` was adjusted so relative links work from inside `docs/` and its SQLite note matches the implemented `EnsureCreated()` flow.

## Files Modified

- `src/GbIrl.Core/Services/IrlStatisticsService.cs`
- `tests/GbIrl.Spec.Tests/Core/IrlStatisticsServiceTests.cs`
- `README.md`
- `docs/TESTS.md`
- `docs/STACK.md`
- `docs/TASKS.md`

## Verification

```bash
dotnet build -m:1
```

Result: build succeeded, 0 warnings, 0 errors.

```bash
dotnet test
```

Result:

```text
Passed!  - Failed:     0, Passed:    27, Skipped:     0, Total:    27, Duration: 972 ms - GbIrl.Spec.Tests.dll (net8.0)
```

## What I Did Not Change

- I did not alter the upload, persistence, filter, edit, or Razor Page routes.
- I did not add migrations, Docker, authentication, export, or import history.
- I did not remove or weaken assessment coverage; I corrected formula expectations to match the documented assessment/test contract.
- I did not run a browser-based manual upload demo in this QA pass; verification was by code audit, build, and automated tests.

## Next Step

Boss can consider the QA findings resolved. The project is still green at 27/27 tests, and the formula implementation now matches the documented rubric instead of relying on fixture-specific behavior.
