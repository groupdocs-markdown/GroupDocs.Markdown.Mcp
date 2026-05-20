using GroupDocs.Mcp.Core;
using GroupDocs.Mcp.Core.Licensing;
using GroupDocs.Markdown.Mcp.Tools;
using Moq;
using Xunit;

namespace GroupDocs.Markdown.Mcp.Tests;

public class ConvertToMarkdownToolTests
{
    private readonly Mock<IFileResolver> _resolver = new();
    private readonly Mock<ILicenseManager> _licenseManager = new();
    private readonly Mock<IFileStorage> _storage = new();
    private readonly OutputHelper _output;

    public ConvertToMarkdownToolTests()
    {
        _output = new OutputHelper(_storage.Object, Microsoft.Extensions.Options.Options.Create(new McpConfig()));
    }

    [Fact]
    public async Task ConvertToMarkdown_WhenResolverThrows_PropagatesException()
    {
        // resolver.ResolveAsync runs OUTSIDE the engine try/catch, so a
        // file-not-found is a caller error and must propagate unchanged.
        _resolver
            .Setup(r => r.ResolveAsync(It.IsAny<FileInput>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new FileNotFoundException("missing.pdf"));

        var ex = await Assert.ThrowsAsync<FileNotFoundException>(() =>
            ConvertToMarkdownTool.ConvertToMarkdown(
                _resolver.Object,
                _storage.Object,
                _licenseManager.Object,
                _output,
                new FileInput { FilePath = "missing.pdf" }));

        Assert.Contains("missing.pdf", ex.Message);
    }

    [Fact]
    public async Task ConvertToMarkdown_SetsLicense_BeforeResolving()
    {
        var sequence = new List<string>();

        _licenseManager
            .Setup(l => l.SetLicense())
            .Callback(() => sequence.Add("license"));

        _resolver
            .Setup(r => r.ResolveAsync(It.IsAny<FileInput>(), It.IsAny<CancellationToken>()))
            .Callback(() => sequence.Add("resolve"))
            .ThrowsAsync(new InvalidOperationException("short-circuit"));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            ConvertToMarkdownTool.ConvertToMarkdown(
                _resolver.Object,
                _storage.Object,
                _licenseManager.Object,
                _output,
                new FileInput { FilePath = "anything.pdf" }));

        Assert.Equal(new[] { "license", "resolve" }, sequence);
    }

    [Fact]
    public async Task ConvertToMarkdown_PassesFileInputToResolver_Unchanged()
    {
        var input = new FileInput { FilePath = "report.docx" };
        FileInput? captured = null;

        _resolver
            .Setup(r => r.ResolveAsync(It.IsAny<FileInput>(), It.IsAny<CancellationToken>()))
            .Callback<FileInput, CancellationToken>((fi, _) => captured = fi)
            .ThrowsAsync(new InvalidOperationException("short-circuit"));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            ConvertToMarkdownTool.ConvertToMarkdown(
                _resolver.Object,
                _storage.Object,
                _licenseManager.Object,
                _output,
                input));

        Assert.Same(input, captured);
    }
}
