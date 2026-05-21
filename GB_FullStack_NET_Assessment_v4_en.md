Senior Full-Stack .NET Engineer — Technical Assessment
Business Context — Read First

Gordon Brothers (GB) is an asset appraisal and liquidation firm. When a company is in distress, GB visits the company, surveys its inventory, and assigns a liquidation value to each item so that a bank can use that inventory as collateral.

The list of surveyed items is called an IRL (Item Record List). It is a spreadsheet where each row is an item with its basic attributes: SKU, description, category, quantity, unit cost, and condition.

The value assigned to each item is called OLV (Orderly Liquidation Value) — how much that item is worth if sold in an orderly liquidation. It is calculated with a simple formula:

OLV = unit_cost * quantity * factor_by_condition

where factor_by_condition: New = 0.70, Used = 0.50, Damaged = 0.20.

Scope
Estimated work time: ~3 hours
Delivery: 24–48 hours from receipt
What we evaluate: your ability to create a .NET project from scratch, choose the supporting stack, integrate several things at once, and deliver something that runs. If you do not finish everything, say so in the README — we prefer honesty over smoke and mirrors.
Stack
Required: .NET 8 or the latest LTS version
Everything else is up to you: ORM, database, Excel parsing library, UI framework — Razor, Blazor, MVC, or whatever you prefer — authentication, and anything else you need.

Briefly justify in the README what you chose and why.

Important: using LLMs/OpenAI/Anthropic inside the app is not allowed, for example, parsing the Excel with AI or calculating statistics with AI. Those calculations must be done with code or standard libraries. For writing the code, you may — and are expected to — use Copilot, Cursor, Claude, or whatever tools you have. We want to see well-executed vibecoding.

How We Run It

Your README must hinetell us how to run it on the reviewer’s mac. Reasonable options include:

dotnet run and that’s it — the simplest option
docker compose up — if you prefer to isolate dependencies
A combination with clear instructions — build, migrate, seed, run

We do not care which option you choose. We care that it is documented and that it actually starts by following the README steps. If you had to take any shortcuts — for example, hardcoding the Excel path, leaving a user in the code, skipping migrations — say so in the README. That helps rather than hurts.

Functionality to Deliver

We provide a sample Excel file, sample_irl.xlsx, with around 30 items containing SKU, description, category, quantity, unit cost, and condition.

The app must:

Import the Excel from an upload screen. Persist the items. Show a post-upload summary: loaded / rejected, with reasons.
List the items in a table with SKU, description, category, quantity, unit cost, condition, and OLV calculated per row.
Edit an item — quantity, unit cost, and condition. When saved, the OLV must be recalculated.
Show the total OLV of the IRL — the sum of OLV for all rows.
Show the weighted average unit cost, weighted by quantity. If you do not know what a weighted average is, research it — it is part of the exercise.
Show the median unit cost of the inventory.
Show the standard deviation of the unit cost of the inventory.
Filter the table by category and condition. When a filter is applied, stats 4–7 must be recalculated based on the visible subset.

If you do not finish everything, prioritize. In the README, explain what you left out and why.

Deliverable
Git repo, preferred, or zip file
README with:
How to run it, step by step, without omitting anything
The stack you chose and why
What you left out and why
Answers to the Q&A below
Q&A — Answer Briefly in the README
Which library did you use to parse the Excel file? Why that one?
Tell us how you approached the statistical calculations — weighted average, median, standard deviation: what formulas you applied and what decisions you made.
Which decision was the hardest during the exercise?
What did you leave out? If you had 3 more hours, what would you add first?
Which part of the code makes you unsure or does not fully satisfy you?
Final Notes
We are not looking for perfect code. We are looking for honest code.
If something frustrated you or you did not solve it well, say so. That counts for more than hiding it.
We will run the project live during the interview. Be prepared to explain any line of code.