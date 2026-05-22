# Subjective Multi-Agent Communication Protocol

This document defines the protocol by which AI chat agents (LLMs) collaborate on the Subjective codebase. Any agent — regardless of provider or model — can join the conversation by following these rules.

---

## 0. Project instance: `kopius-gb-fullstack-net-assessment`

This copy applies to the **Gordon Brothers IRL** .NET assessment repo.

| Setting | Value |
|---------|--------|
| **Messages directory** | `messages/` at repo root (this folder) |
| **Project root** | Parent of `messages/` |
| **Agents on this project** | `boss` (coordination), `coder` (implementation) |
| **Primary thread** | `gb_irl_implementation` |
| **Definition of done** | `dotnet test` → 27/27 pass; README complete per assessment |

Subjective-specific paths in Sections 4–8 (Windows logs, `c:\brainboost\Subjective`, etc.) apply only when working on Subjective. For this repo, use `dotnet test` output and repo docs instead of Subjective logs.

---

## 1. Message File Convention

All inter-agent messages are stored in this directory:

```
messages/
```

### File Naming

```
YYYY_MM_DD_HH_MM_SS-<sender>-to-<receiver>-<thread>-message.md
```

| Field | Format | Description |
|-------|--------|-------------|
| `YYYY_MM_DD_HH_MM_SS` | Timestamp | UTC or local time of message creation. Use underscores, not dashes, between date/time components. |
| `<sender>` | Lowercase identifier | The agent writing the message (e.g., `claude`, `codex`, `gemini`, `llama`, `gpt`, `boss`, `coder`). |
| `<receiver>` | Lowercase identifier | The agent the message is addressed to. Use `all` to broadcast to every agent in the thread. |
| `<thread>` | Lowercase snake_case slug | A short topic name that groups related messages into a conversation thread. See **Thread Naming** below. |

### Thread Naming

Threads keep messages about different topics separated. The **first agent** to raise a new topic creates the thread name. All subsequent messages on that topic reuse the exact same thread name.

**Rules:**

1. **Creating a thread** — When starting a new topic, pick a short, descriptive `snake_case` slug (e.g., `redis_timeout_bug`, `pipeline_ui_rework`, `auth_refactor`, `gb_irl_implementation`). Keep it to 2–4 words. You are now the thread creator.
2. **Replying to a thread** — Reuse the exact `<thread>` value from the message you are responding to. Do not rename or paraphrase it.
3. **General messages (no thread)** — If the message is not tied to any specific topic (e.g., introductions, broad status updates), use `general` as the thread name.

### Reserved Receiver: `all`

When `<receiver>` is set to `all`, the message is a **broadcast** intended for every agent participating in the thread. Use this for:

- Announcements that affect all agents (e.g., architecture decisions, convention changes, project-wide status updates)
- Shared context that every agent needs before continuing their work
- Coordination messages when multiple agents are working in parallel

Every agent must treat `all` messages as directed at them and read them before writing their next response.

Examples:

```
2026_03_21_18_55_15-claude-to-codex-redis_timeout_bug-message.md   # threaded direct message
2026_03_21_16_49_18-codex-to-claude-redis_timeout_bug-message.md   # reply in same thread
2026_03_22_09_00_00-gemini-to-claude-pipeline_ui_rework-message.md # different thread
2026_03_22_10_30_00-claude-to-all-general-message.md               # broadcast, general topic
2026_03_22_11_00_00-claude-to-all-redis_timeout_bug-message.md     # broadcast within a thread
```

### Sorting and Threading

Messages are sorted chronologically by filename. The timestamp prefix guarantees chronological order. The `<thread>` segment allows filtering by topic so conversations on different subjects do not mix.

To find all messages in a specific thread:

```bash
ls -1 messages/ | grep "gb_irl_implementation-message"
```

To find the latest message from a specific agent:

```bash
ls -1 messages/*-<agent_name>-to-*-message.md | tail -1
```

To find all messages addressed to you (including broadcasts), across all threads:

```bash
ls -1 messages/ | grep -E "to-(your_name|all)-"
```

To find all messages addressed to you in a specific thread:

```bash
ls -1 messages/ | grep -E "to-(your_name|all)-gb_irl_implementation-message"
```

To list all active threads:

```bash
ls -1 messages/*-message.md | sed 's/.*-to-[^-]*-//;s/-message\.md//' | sort -u
```

---

## 2. Message File Structure

Every message file is a standard Markdown document. Use this structure:

```markdown
# <Sender> to <Receiver>: <Short Title>

## Context Check

I checked the latest thread message and the latest external logs before writing this.

Latest <other agent> message read:
- `<filename of last message you read>`

Latest external logs at reply time:
- `<newest relevant log file(s)>`

## <Body Sections>

Organize the body by topic. Use sections like:
- What I Implemented
- What I Found
- Root Cause Analysis
- Files Modified
- Tests Added / Run
- Verification
- What I Did Not Change
- Next Step
```

### Rules

1. **MANDATORY — Read before writing.** Before elaborating any answer, implementing any change, or writing any response message, you MUST first read ALL unread messages addressed to you or to `all`. List the files in this directory, filter for messages where `<receiver>` matches your agent name or `all`, and read every one you have not yet acknowledged. Do not skip this step. Do not assume you are up to date. If there are no new messages, state that explicitly in your Context Check.

   ```bash
   # Run this FIRST, before doing anything else — find all messages for you across all threads
   ls -1 messages/ | grep -E "to-(your_name|all)-"

   # Or, to focus on a specific thread:
   ls -1 messages/ | grep -E "to-(your_name|all)-thread_name-message"
   ```

2. **State what you read.** In the Context Check section of your response message, list every message file you read and confirm you incorporated its contents into your work.
3. **Always check the latest logs before writing.** State which log files you reviewed. (For this GB repo: paste `dotnet test` summary.)
4. **Be specific.** Reference exact file paths, line numbers, function names, class names, and error messages.
5. **Include test results.** If you ran tests, paste the command and result summary.
6. **State what you did NOT change.** This prevents the other agent from assuming you handled something you didn't.
7. **End with a clear Next Step.** The receiving agent should know exactly what to do next.

---

## 3. Agent Identity

When joining the conversation for the first time, an agent should:

1. Choose a short, lowercase identifier (e.g., `claude`, `codex`, `gemini`, `llama`, `gpt`, `mistral`, `boss`, `coder`).
2. Use that identifier consistently in all filenames.
3. In the first message, briefly state which model/version you are and what tools you have access to (file editing, code execution, web search, etc.).

---

## 4. Project Logs

Runtime logs are generated by the Subjective application and its tools. Always check the latest logs before replying to understand the current runtime state.

### Log Location

```
C:\Users\pablo\.Subjective\com_subjective_userdata\com_subjective_logs
```

### Log File Naming

```
YYYY_MM_DD_HH_MM_SS-<process_name>-log.log
```

Examples:

```
2026_03_21_15_46_23-main-log.log
2026_03_21_15_46_31-Llama-llanma_local-log.log
2026_03_21_15_46_31-AnthropicClaude-claude_opus_4_5-log.log
2026_03_18_10_22_05-pipeline_editor-log.log
```

### How to Use Logs

- Logs are named `per_run` — each application launch produces a new log file.
- The `main-log.log` file contains the main application's output.
- Data source and tool logs are named by their process/plugin name.
- The `pipeline_editor-log.log` files contain standalone pipeline editor output, including `ICON TRACE` and `PORT TRACE` diagnostic lines when debug mode is enabled.
- To find the latest log: sort by filename (timestamp prefix guarantees chronological order).
- Log columns: `timestamp | log_type | process | code_location | message | processing_time` (pipe-delimited).

---

## 5. Context Files

Context files contain structured data captured by data sources during processing runs. These are useful for understanding what data the application has processed.

### Context Location

```
C:\Users\pablo\.Subjective\com_subjective_userdata\com_subjective_context
```

### Context File Naming

```
YYYY_MM_DD_HH_MM_SS-<ds_name>-context.json
```

### What Context Files Contain

Each context file is a JSON document with the output of a data source processing run. The structure varies by data source type but generally includes the processed data, metadata, timestamps, and source identifiers.

---

## 6. User Data Root

The user data directory is the base path for all user-specific data:

```
C:\Users\pablo\.Subjective\com_subjective_userdata
```

Key subdirectories:

| Path | Purpose |
|------|---------|
| `com_subjective_logs` | Runtime logs (see Section 4) |
| `com_subjective_context` | Context output files (see Section 5) |
| `com_subjective_plugins` | Installed data source plugins |
| `com_subjective_pipelines` | Saved pipeline `.pipe` files |
| `com_subjective_data` | Data source persistent storage |
| `com_subjective_snapshots` | Application snapshots |
| `com_subjective_environment` | uv-managed Python environment for data sources |

---

## 7. Codebase Location and Structure

The project root is:

```
c:\brainboost\Subjective
```

Key directories:

| Path | Purpose |
|------|---------|
| `com_subjective_ui/` | PyQt5 desktop application UI code |
| `com_subjective_themes/` | Theme files (icons, colors) |
| `com_subjective_tools/` | Standalone tool executables (pipeline editor, log viewer, etc.) |
| `com_subjective_architecture_docs/` | Architecture documentation and this messages folder |
| `com_subjective_architecture_docs/messages/` | Inter-agent message files (this protocol) |
| `scripts/` | Utility scripts, including `datasources.json` plugin registry |
| `tests/` | Test suite |

**GB IRL assessment repo** (this copy): see repo root `README.md`, `src/`, `tests/GbIrl.Spec.Tests/`, `samples/`.

---

## 8. Configuration Architecture Rule

**Standalone tools NEVER read `subjective.conf` directly.** All configuration flows through CLI arguments:

```
subjective.conf --> BBConfig (main app) --> --arg value (CLI) --> standalone tool
```

The main configuration file is at the project root: `subjective.conf`. The main application reads it into `BBConfig`. When launching standalone tools as subprocesses, the main app passes relevant settings as command-line arguments (e.g., `--redis-ip`, `--redis-port`, `--userdata-path`, `--debug`).

---

## 9. Workflow Summary

When an agent receives a task or picks up a conversation:

1. **FIRST — Read all pending messages.** Before doing anything else, list all files in `messages/` and read every message where `<receiver>` is your agent name or `all` that you have not yet processed. This is non-negotiable — never skip this step, never assume you are caught up.
2. **Check the latest logs** in the user data logs folder to understand runtime behavior. (GB repo: run `dotnet test`.)
3. **Read relevant source files** before proposing changes.
4. **Implement changes** with specific file edits, not vague suggestions.
5. **Run tests** and include results in your message.
6. **Write a response message** following the naming convention and structure defined above.
7. **State your next step** so the conversation can continue.

---

*Source: `com_subjective_architecture_docs/messages/agents_protocol.md` (Subjective). Adapted for GB IRL assessment with Section 0 overlay.*
