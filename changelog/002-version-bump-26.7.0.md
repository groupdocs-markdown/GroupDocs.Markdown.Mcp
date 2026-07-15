---
id: 002
date: 2026-07-15
version: 26.7.0
type: chore
---

# Release 26.7.0 — version bump + conventions refresh

## What changed
- MCP package version bumped `26.5.0 → 26.7.0` (CalVer) across
  `build/dependencies.props`, `.mcp/server.json` (both `version` fields), and the
  `@<version>` doc pins in `README.md` and `llms.txt`.
- No engine change: GroupDocs.Markdown stays at **26.3.0** — still the latest
  stable on NuGet (0.0.0/25.9.0/26.1.0/26.3.0). SkiaSharp native-asset pin
  (`SkiaSharp.NativeAssets.Linux.NoDependencies` 3.119.0) unchanged.
- No tool-surface change: still `ConvertToMarkdown` + `GetDocumentInfo` (2 tools).
  Both tools already wrap engine calls in `try/catch` returning a descriptive
  `"<Op> failed for '<file>': <Type>: <msg> | inner(n): …"` string.

## Why
Keeps GroupDocs.Markdown.Mcp in lockstep with the cross-product MCP release train
(Metadata / Parser / Conversion at 26.7.x). Version-only maintenance release.

## Migration / impact
None — no API, tool, or behaviour change. Consumers pinned to `@26.5.0` continue
to work; `@26.7.0` is a drop-in replacement.
