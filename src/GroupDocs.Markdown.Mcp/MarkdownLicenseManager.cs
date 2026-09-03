using GroupDocs.Mcp.Core;
using GroupDocs.Mcp.Core.Licensing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GroupDocs.Markdown.Mcp;

public class MarkdownLicenseManager : LicenseManager
{
    public MarkdownLicenseManager(IOptions<McpConfig> config, ILogger<LicenseManager> logger)
        : base(config, logger)
    {
    }

    // Identifies the engine in get_license_status. Without it the tool would report the
    // server's own version, because this class lives in the server assembly.
    protected override Type? EngineMarkerType => typeof(GroupDocs.Markdown.License);

    protected override void SetLicenseFromPath(string licensePath)
    {
        new GroupDocs.Markdown.License().SetLicense(licensePath);
    }

    protected override void SetMeteredKeyCore(string publicKey, string privateKey)
    {
        new GroupDocs.Markdown.Metered().SetMeteredKey(publicKey, privateKey);
    }

    protected override MeteredConsumption ReadConsumptionCore()
    {
        // Static on the engine and only meaningful once a metered key is applied - Core
        // guarantees this runs in metered mode only.
        return new MeteredConsumption
        {
            Quantity = GroupDocs.Markdown.Metered.GetConsumptionQuantity(),
            Credit = GroupDocs.Markdown.Metered.GetConsumptionCredit()
        };
    }
}
