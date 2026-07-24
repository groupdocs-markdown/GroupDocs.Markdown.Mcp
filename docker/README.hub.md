# GroupDocs.Markdown MCP Server

MCP server that exposes [GroupDocs.Markdown](https://products.groupdocs.com/markdown) as AI-callable tools for Claude, Cursor, GitHub Copilot, and other MCP agents — convert documents to clean, structured Markdown.

## Quick start

```bash
docker run --rm -i \
  -v $(pwd)/documents:/data \
  groupdocs/markdown-net-mcp:latest
```

## Use with Claude Desktop

```json
{
  "mcpServers": {
    "groupdocs-markdown": {
      "command": "docker",
      "args": ["run", "--rm", "-i", "-v", "/path/to/documents:/data", "groupdocs/markdown-net-mcp:latest"]
    }
  }
}
```

## Tools

- **ConvertToMarkdown** — Converts a document (PDF, DOCX, XLSX, EPUB, MOBI, …) to clean, structured Markdown and saves the `.md` to storage. Images embed as base64 by default.
- **GetDocumentInfo** — Returns file format, page count, title, author, and encryption flag as JSON, without converting.

## Tags & environment

- Tags: `latest` + an immutable version tag per release matching NuGet (e.g. `26.7.1`).
  Platforms: `linux/amd64`, `linux/arm64`. Also on GHCR: `ghcr.io/groupdocs-markdown/markdown-net-mcp`.
- `GROUPDOCS_MCP_STORAGE_PATH` (default `/data`), `GROUPDOCS_MCP_OUTPUT_PATH` (optional),
  `GROUPDOCS_LICENSE_PATH` — mount your license and point at it to leave evaluation mode
  (see the Licensing section in the GitHub README for the exact evaluation limits).

Full docs, one-click installs for other clients, and licensing details:
**https://github.com/groupdocs-markdown/GroupDocs.Markdown.Mcp**
