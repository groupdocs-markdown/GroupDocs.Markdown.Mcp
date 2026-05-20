using System.ComponentModel;
using System.Text;
using System.Text.Json;
using GroupDocs.Mcp.Core;
using GroupDocs.Mcp.Core.Licensing;
using ModelContextProtocol.Server;

namespace GroupDocs.Markdown.Mcp.Tools;

[McpServerToolType]
public static class GetDocumentInfoTool
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    [McpServerTool, Description(
        "Returns document information (file format, page count, title, author, encryption flag) as JSON, without performing a full Markdown conversion. " +
        "Supports PDF, DOCX, XLSX, EPUB, MOBI, and 20+ more formats recognized by GroupDocs.Markdown. " +
        "Useful as a precondition check before ConvertToMarkdown — e.g. 'how many pages does this PDF have?' or to decide which pages to request or whether a password is needed. " +
        "Returns a JSON object with `fileName`, `fileFormat`, `pageCount` (worksheet count for spreadsheets), `title`, `author`, and `isEncrypted`. " +
        "Do NOT pre-check whether files exist — just pass the filename the user provided. " +
        "The tool resolves files from storage and returns an error with available files if a name is not found. " +
        "On failure, the response text starts with 'Document-info lookup failed for' followed by the underlying exception type, message, and inner chain.")]
    public static async Task<string> GetDocumentInfo(
        IFileResolver resolver,
        ILicenseManager licenseManager,
        FileInput file,
        [Description("Password for protected documents")] string? password = null)
    {
        licenseManager.SetLicense();
        using var resolved = await resolver.ResolveAsync(file);

        try
        {
            var loadOptions = new LoadOptions { Password = password };
            using var converter = new MarkdownConverter(resolved.Stream, loadOptions);
            var info = converter.GetDocumentInfo();

            var result = new
            {
                fileName = resolved.FileName,
                fileFormat = info.FileFormat.ToString(),
                pageCount = info.PageCount,
                title = info.Title,
                author = info.Author,
                isEncrypted = info.IsEncrypted
            };

            return JsonSerializer.Serialize(result, JsonOptions);
        }
        catch (Exception ex)
        {
            return FormatException(ex, resolved.FileName);
        }
    }

    private static string FormatException(Exception ex, string fileName)
    {
        var sb = new StringBuilder();
        sb.Append($"Document-info lookup failed for '{fileName}': ");
        sb.Append($"{ex.GetType().FullName}: {ex.Message}");
        var inner = ex.InnerException;
        for (int depth = 0; inner != null && depth < 5; depth++, inner = inner.InnerException)
            sb.Append($" | inner({depth}): {inner.GetType().FullName}: {inner.Message}");
        return sb.ToString();
    }
}
