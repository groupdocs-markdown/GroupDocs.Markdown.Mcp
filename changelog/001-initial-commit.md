---
id: 001
date: 2026-05-19
version: 26.5.0
type: feature
---

# Initial public release of GroupDocs.Markdown MCP Server

## What changed
- NuGet package `GroupDocs.Markdown.Mcp` published with the `McpServer` package type.
- Two MCP tools exposed:
  - `ConvertToMarkdown` — converts a document (PDF, DOCX, XLSX, EPUB, MOBI, and 20+ more formats)
    to clean, structured Markdown. Images embed as base64 by default; `images` switches to
    file output or skip. Supports `pages`, `frontMatter`, `flavor` (github/commonmark), and
    `password`. Saves the `.md` to storage and returns the content inline.
  - `GetDocumentInfo` — returns file format, page count, title, author, and encryption flag
    as JSON, without converting. Cross-product standard tool (Step 11).
- Installable via `dnx GroupDocs.Markdown.Mcp@26.5.0 --yes` (.NET 10 SDK required) or
  `dotnet tool install -g GroupDocs.Markdown.Mcp` (CLI command `groupdocs-markdown-mcp`).
- Docker image published to `ghcr.io/groupdocs-markdown/markdown-net-mcp` and
  `docker.io/groupdocs/markdown-net-mcp`.
- Environment variables: `GROUPDOCS_MCP_STORAGE_PATH`, optional `GROUPDOCS_MCP_OUTPUT_PATH`,
  `GROUPDOCS_LICENSE_PATH`.

## Tools NOT exposed (and why)
- `ComposeFromMarkdown` (reverse Markdown→document composition): both
  `MarkdownConverter.FromMarkdownString` overloads in GroupDocs.Markdown 26.3.0
  (the latest stable on NuGet) throw `NotImplementedException` with the message
  *"Reverse conversion (Markdown to Document) will be available in a future release."*
  Rather than ship a tool whose only response is an error, the server omits it.
  When the engine ships reverse conversion, add `ComposeFromMarkdownTool` together
  with its unit test + integration test and bump the tool count in
  `ToolDiscoveryTests`. In the meantime, the
  [GroupDocs.Conversion MCP](https://www.nuget.org/packages/GroupDocs.Conversion.Mcp)
  produces DOCX / PDF / HTML from a `.md` source file.

## Why
Adds GroupDocs.Markdown to the GroupDocs MCP framework (cloned from the GroupDocs.Metadata
MCP server). Exposes document-to-Markdown conversion as an AI-callable tool for Claude,
Cursor, VS Code / GitHub Copilot, and other MCP-compatible agents.

## Native-dependency decisions
- **SkiaSharp**: GroupDocs.Markdown 26.3.0 renders image-bearing documents through SkiaSharp
  (declared in the upstream `GroupDocs.Markdown.Net*.nuspec`). The csproj pins
  `SkiaSharp.NativeAssets.Linux.NoDependencies` (3.119.0) for deterministic transitive
  resolution. The `.NoDependencies` native asset is self-contained, so the Docker image and
  bare `dnx`/global-tool runs need **no** extra `apt`/`brew` packages.
- **System.Drawing**: GroupDocs.Markdown does **not** use `System.Drawing`/GDI+. The
  `System.Drawing.EnableUnixSupport` runtime flag and the `libgdiplus`/`libfontconfig1`
  Dockerfile `apt-get` block (carried by the Metadata template) were removed.

## GetDocumentInfo output format
The framework subproject's `GetDocumentInfoTool` returned a plaintext block. For
cross-product alignment with Conversion / Comparison / Viewer / Watermark, this clone
returns a JSON object instead (`fileName`, `fileFormat`, `pageCount`, `title`, `author`,
`isEncrypted`).

## TODO when GroupDocs.Markdown ships reverse conversion
- [ ] Re-introduce `ComposeFromMarkdownTool` (the framework subproject has a scaffold to
      port from). Add `ComposeFromMarkdownToolTests` (main repo) and
      `ComposeFromMarkdownTests` (Tests repo) at the same time.
- [ ] Update `ToolCatalog` (add `ComposeFromMarkdown => Resolve("compose")`) and the
      `ListTools_*` assertion in `ToolDiscoveryTests` (2 → 3).
- [ ] Update README / AGENTS / llms.txt tool tables + Dockerfile OCI label + this changelog
      note + the "Tools NOT exposed" section above (delete it or move to a fixed-in note).

## Migration / impact
First release — no migration required.
