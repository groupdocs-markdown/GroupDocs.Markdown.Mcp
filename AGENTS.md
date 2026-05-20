# AGENTS.md — Guide for AI coding agents

Brief orientation for AI coding agents (Claude Code, Copilot, Cursor, Aider, Amp, Codex) working in this repository.

## What this repo is

A standalone **MCP server** for [GroupDocs.Markdown for .NET](https://products.groupdocs.com/markdown) — exposes document-to-Markdown conversion as AI-callable tools via the Model Context Protocol.

Published to NuGet as `GroupDocs.Markdown.Mcp` with the `McpServer` package type, and to `ghcr.io/groupdocs-markdown/markdown-net-mcp` + `docker.io/groupdocs/markdown-net-mcp` as a container image.

## MCP tools exposed

| Tool | Description |
|---|---|
| `ConvertToMarkdown` | Convert a document (PDF, DOCX, XLSX, EPUB, MOBI, …) to clean, structured Markdown; save the `.md` to storage and return the content inline. Supports `images` (base64/file/skip), `pages`, `frontMatter`, `flavor`. |
| `GetDocumentInfo` | Return file format, page count, title, author, and encryption flag as JSON, without converting. |

Both tools accept `FileInput` (resolved via `IFileResolver`) and an optional `password` for protected documents.

> **No `ComposeFromMarkdown` tool.** Reverse Markdown → document composition
> (`MarkdownConverter.FromMarkdownString`) throws `NotImplementedException` in
> GroupDocs.Markdown 26.3.0 — so the server does not advertise the reverse
> direction. When the engine ships reverse conversion, add a
> `ComposeFromMarkdownTool` + unit test + integration test together (the
> framework subproject already has a scaffold to port from).

## Folder layout

```
src/                                           ← all projects + sln + Directory.Build.props
  GroupDocs.Markdown.Mcp/
    Program.cs                                 ← host bootstrap + stdio transport
    MarkdownLicenseManager.cs                  ← applies GroupDocs.Total license
    Tools/
      ConvertToMarkdownTool.cs                 ← [McpServerTool] — ConvertToMarkdown
      GetDocumentInfoTool.cs                   ← [McpServerTool] — GetDocumentInfo
    .mcp/
      server.json                              ← NuGet.org reads this to generate mcp.json snippet
    GroupDocs.Markdown.Mcp.csproj              ← PackageType=McpServer + ToolCommandName
  GroupDocs.Markdown.Mcp.Tests/                ← xUnit unit tests (Moq)
  GroupDocs.Markdown.Mcp.sln
  Directory.Build.props
build/
  dependencies.props                           ← single source of truth for all versions
changelog/                                     ← one MD file per change (see changelog/README.md)
docker/
  Dockerfile                                   ← multi-stage, runtime on aspnet:10.0
  docker-compose.yml
.github/workflows/                             ← build_packages.yml, run_tests.yml, publish_prod.yml, publish_docker.yml
```

## Dependencies

- `GroupDocs.Mcp.Core` + `GroupDocs.Mcp.Local.Storage` — infrastructure NuGet packages from the [GroupDocs.Mcp.Core](https://github.com/groupdocs/GroupDocs.Mcp.Core) repo
- `GroupDocs.Markdown` — the actual document-to-Markdown conversion engine
- `SkiaSharp.NativeAssets.Linux.NoDependencies` — self-contained native asset for image-bearing conversions on Linux
- `ModelContextProtocol` — MCP SDK for .NET
- `Microsoft.Extensions.Hosting` — host builder for the stdio server

## Commands you can run

```bash
# Restore + build
dotnet restore
dotnet build src/GroupDocs.Markdown.Mcp.sln -c Release

# Run tests
dotnet test src/GroupDocs.Markdown.Mcp.sln -c Release

# Run the server locally (stdio)
dotnet run --project src/GroupDocs.Markdown.Mcp

# Local pack (writes to ./build_out) — validates server.json version matches dependencies.props
pwsh ./build.ps1

# Build + run the Docker image
docker build -f docker/Dockerfile -t markdown-net-mcp:local .
docker run --rm -i -v $(pwd)/documents:/data markdown-net-mcp:local
```

## Version scheme

CalVer `YY.MM.N`. The version lives in **two** places that MUST stay in lockstep:
1. `build/dependencies.props` → `<GroupDocsMarkdownMcp>`
2. `src/GroupDocs.Markdown.Mcp/.mcp/server.json` → both top-level `"version"` and `packages[0].version`

`build.ps1` enforces this at pack time (`Assert-ServerJsonVersionMatchesDependencies`) — if they drift, the build fails.

## House rules

1. **Tools must have rich `[Description("...")]` strings** — these are what AI agents read via the MCP protocol. Write them as task-oriented sentences, not method-signature summaries.
2. **Never add new env vars beyond** `GROUPDOCS_MCP_STORAGE_PATH`, `GROUPDOCS_MCP_OUTPUT_PATH`, `GROUPDOCS_LICENSE_PATH` without updating `server.json`, `docker-compose.yml`, and `README.md` together.
3. **Tests use xUnit + Moq** — mock `IFileResolver`, `IFileStorage`, `ILicenseManager`, `OutputHelper`.
4. **Changelog entries required** — any PR that changes behaviour adds `changelog/NNN-slug.md`.
5. **Do not edit `obj/` or `build_out/`** — build artifacts.
6. **Target framework is `net10.0` only** — required by `dnx` and the MCP SDK.

## Release flow

See [RELEASE.md](RELEASE.md) for the exact per-release checklist.

## What NOT to change

- Do not hardcode the version in `.csproj` — it flows from `$(GroupDocsMarkdownMcp)` in `dependencies.props`.
- Do not remove the `<PackageType>McpServer</PackageType>` or `<ToolCommandName>groupdocs-markdown-mcp</ToolCommandName>` from the csproj — NuGet.org discoverability and `dnx` invocation depend on them.
- Do not change the `.mcp/server.json` schema URL without cross-checking with the NuGet MCP docs.
