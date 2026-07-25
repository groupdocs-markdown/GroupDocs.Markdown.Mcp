# Codex CLI (OpenAI)

```bash
codex mcp add groupdocs-markdown -- dnx GroupDocs.Markdown.Mcp --yes
```

Or add to `~/.codex/config.toml`:

```toml
[mcp_servers.groupdocs-markdown]
command = "dnx"
args = ["GroupDocs.Markdown.Mcp", "--yes"]

[mcp_servers.groupdocs-markdown.env]
GROUPDOCS_MCP_STORAGE_PATH = "/path/to/documents"
GROUPDOCS_MCP_OUTPUT_PATH = "/path/to/documents"
GROUPDOCS_LICENSE_PATH = ""   # empty = evaluation mode; set to your GroupDocs.Total.lic to lift limits
```

Pin a version by replacing `GroupDocs.Markdown.Mcp` with `GroupDocs.Markdown.Mcp@26.7.2`.
