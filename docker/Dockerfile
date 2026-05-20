FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish src/GroupDocs.Markdown.Mcp/GroupDocs.Markdown.Mcp.csproj \
    -c Release -o /app --no-self-contained

FROM base AS final

# No extra native packages are required. GroupDocs.Markdown does not use
# System.Drawing/GDI+; its image-bearing conversion paths (PDF/DOCX figures,
# embedded picture encoding) go through SkiaSharp, and the
# SkiaSharp.NativeAssets.Linux.NoDependencies package bundles a self-contained
# libSkiaSharp.so that needs no apt-installed libraries on the Debian-slim
# aspnet:10.0 base image.

# OCI image labels. VERSION is supplied by the publish workflow via
# --build-arg VERSION=YY.MM.N. Default 'dev' makes local `docker build` runs
# self-identify instead of inheriting the base image's misleading version.
ARG VERSION=dev
LABEL org.opencontainers.image.title="GroupDocs.Markdown.Mcp" \
      org.opencontainers.image.description="MCP server exposing GroupDocs.Markdown for .NET as AI-callable tools (ConvertToMarkdown, GetDocumentInfo) for Claude, Cursor, GitHub Copilot, and other MCP agents." \
      org.opencontainers.image.version="${VERSION}" \
      org.opencontainers.image.source="https://github.com/groupdocs-markdown/GroupDocs.Markdown.Mcp" \
      org.opencontainers.image.url="https://www.nuget.org/packages/GroupDocs.Markdown.Mcp" \
      org.opencontainers.image.licenses="MIT" \
      org.opencontainers.image.vendor="Aspose Pty Ltd"

COPY --from=build /app .

ENV GROUPDOCS_MCP_STORAGE_PATH=/data
VOLUME /data

USER $APP_UID

ENTRYPOINT ["dotnet", "GroupDocs.Markdown.Mcp.dll"]
