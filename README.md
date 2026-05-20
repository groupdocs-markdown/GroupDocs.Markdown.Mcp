# GroupDocs.Markdown MCP Server

MCP server that exposes [GroupDocs.Markdown](https://products.groupdocs.com/markdown) as AI-callable tools
for Claude, Cursor, GitHub Copilot, and other MCP agents — convert documents to clean, structured Markdown.

## Installation

Requires [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

**Run directly with `dnx` (recommended — no install step):**

```bash
dnx GroupDocs.Markdown.Mcp --yes
```

Pulls the latest stable release on every invocation. To pin to a specific
version (recommended for shared configs and CI), append `@<version>`:

```bash
dnx GroupDocs.Markdown.Mcp@26.5.0 --yes
```

**Or install as a global dotnet tool:**

```bash
dotnet tool install -g GroupDocs.Markdown.Mcp
groupdocs-markdown-mcp
```

**Or run via Docker:**

```bash
docker run --rm -i \
  -v $(pwd)/documents:/data \
  ghcr.io/groupdocs-markdown/markdown-net-mcp:latest
```

No native prerequisites are required on any platform — the underlying
GroupDocs.Markdown engine renders image-bearing documents through a
self-contained SkiaSharp native asset, so `dnx`, the global tool, and the
Docker image all run with no extra `apt`/`brew` setup.

## Available MCP Tools

| Tool | Description |
|---|---|
| `ConvertToMarkdown` | Converts a document (PDF, DOCX, XLSX, EPUB, MOBI, …) to clean, structured Markdown and saves the `.md` to storage. Images embed as base64 by default. |
| `GetDocumentInfo` | Returns file format, page count, title, author, and encryption flag as JSON, without converting. |

> Reverse Markdown → document composition is **not yet implemented** in the
> underlying GroupDocs.Markdown 26.3.0 engine (`MarkdownConverter.FromMarkdownString`
> throws `NotImplementedException`), so this server does **not** expose a
> `ComposeFromMarkdown` tool. Use the
> [GroupDocs.Conversion MCP](https://www.nuget.org/packages/GroupDocs.Conversion.Mcp)
> with a `.md` source file if you need Markdown → DOCX / PDF / HTML output today.

## Example prompts for AI agents

- "Convert report.pdf to Markdown"
- "Export business-plan.docx as Markdown with images embedded as base64"
- "Convert pages 1-3 of contract.pdf to Markdown, text only — skip images"
- "How many pages does business-plan.pdf have?"
- "What format and author does cost-analysis.xlsx have?"

## Configuration

| Variable | Description | Default |
|---|---|---|
| `GROUPDOCS_MCP_STORAGE_PATH` | Base folder for input and output files | current directory |
| `GROUPDOCS_MCP_OUTPUT_PATH` | *(Optional)* separate folder for output files | `GROUPDOCS_MCP_STORAGE_PATH` |
| `GROUPDOCS_LICENSE_PATH` | Path to GroupDocs license file | (evaluation mode) |

In evaluation mode (no license) the converted Markdown may be limited and can
include an evaluation notice. Supply `GROUPDOCS_LICENSE_PATH` for unrestricted
output.

## Usage with Claude Desktop

```json
{
  "mcpServers": {
    "groupdocs-markdown": {
      "type": "stdio",
      "command": "dnx",
      "args": ["GroupDocs.Markdown.Mcp", "--yes"],
      "env": {
        "GROUPDOCS_MCP_STORAGE_PATH": "/path/to/documents"
      }
    }
  }
}
```

> To pin to a specific version, replace `"GroupDocs.Markdown.Mcp"` with
> `"GroupDocs.Markdown.Mcp@26.5.0"` in `args`. Pinning is recommended for
> shared / committed configs to avoid surprise upgrades.

## Usage with VS Code / GitHub Copilot

NuGet.org generates a ready-to-use `mcp.json` snippet on the [package page](https://www.nuget.org/packages/GroupDocs.Markdown.Mcp).
Copy it directly into your `.vscode/mcp.json`.

Alternatively, add manually to `.vscode/mcp.json`:

```json
{
  "inputs": [
    {
      "type": "promptString",
      "id": "storage_path",
      "description": "Base folder for input and output files.",
      "password": false
    }
  ],
  "servers": {
    "groupdocs-markdown": {
      "type": "stdio",
      "command": "dnx",
      "args": ["GroupDocs.Markdown.Mcp", "--yes"],
      "env": {
        "GROUPDOCS_MCP_STORAGE_PATH": "${input:storage_path}"
      }
    }
  }
}
```

> Same pinning rule as above — swap `"GroupDocs.Markdown.Mcp"` for
> `"GroupDocs.Markdown.Mcp@26.5.0"` to lock to a specific release.

## Usage with Docker Compose

```bash
cd docker
docker compose up
```

Edit `docker/docker-compose.yml` to point volumes at your local documents folder.

## Documentation & guides

Step-by-step deployment guides and a published-package integration test suite
live in the companion repo
[**GroupDocs.Markdown.Mcp.Tests**](https://github.com/groupdocs-markdown/GroupDocs.Markdown.Mcp.Tests):

- [Install from NuGet](https://github.com/groupdocs-markdown/GroupDocs.Markdown.Mcp.Tests/blob/master/how-to/01-install-from-nuget.md) — `dnx`, global tool, pinned vs always-latest
- [Run via Docker](https://github.com/groupdocs-markdown/GroupDocs.Markdown.Mcp.Tests/blob/master/how-to/02-run-via-docker.md)
- [Verify on the MCP registry](https://github.com/groupdocs-markdown/GroupDocs.Markdown.Mcp.Tests/blob/master/how-to/03-verify-mcp-registry.md)
- [Use with Claude Desktop](https://github.com/groupdocs-markdown/GroupDocs.Markdown.Mcp.Tests/blob/master/how-to/04-use-with-claude-desktop.md)
- [Use with VS Code / GitHub Copilot](https://github.com/groupdocs-markdown/GroupDocs.Markdown.Mcp.Tests/blob/master/how-to/05-use-with-vscode-copilot.md)
- [Run the integration tests](https://github.com/groupdocs-markdown/GroupDocs.Markdown.Mcp.Tests/blob/master/how-to/06-run-integration-tests.md)

That repo also exercises every advertised tool against the **published** NuGet
artifact on Linux, macOS, and Windows in CI — so the snippets above are
verified end-to-end on every release.

## License

MIT — see [LICENSE](LICENSE)

<!-- mcp-name: io.github.groupdocs-markdown/groupdocs-markdown-mcp -->
