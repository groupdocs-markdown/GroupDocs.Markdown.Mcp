using GroupDocs.Mcp.Core;
using GroupDocs.Mcp.Core.Licensing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GroupDocs.Markdown.Mcp;

public class MarkdownLicenseManager : LicenseManager
{
    public MarkdownLicenseManager(IOptions<McpConfig> config, ILogger<LicenseManager> logger) : base(config, logger) { }
    protected override void SetLicenseFromPath(string licensePath)
    {
        new GroupDocs.Markdown.License().SetLicense(licensePath);
    }
}
