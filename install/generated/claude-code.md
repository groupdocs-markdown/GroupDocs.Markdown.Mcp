# Claude Code

```bash
claude mcp add groupdocs-markdown -- dnx GroupDocs.Markdown.Mcp --yes
```

With storage folder and license:

```bash
claude mcp add groupdocs-markdown -e GROUPDOCS_MCP_STORAGE_PATH=/path/to/documents -e GROUPDOCS_LICENSE_PATH=/path/to/GroupDocs.Total.lic -- dnx GroupDocs.Markdown.Mcp --yes
```

Pin a version by replacing `GroupDocs.Markdown.Mcp` with `GroupDocs.Markdown.Mcp@26.7.2`.
