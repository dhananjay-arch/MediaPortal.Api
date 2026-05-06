namespace MediaPortal.Api.Models;

public class DashboardPayload
{
    public IEnumerable<MediaBuyRecord> MediaBuy { get; set; } = [];
    public IEnumerable<ConversionRecord> Conversions { get; set; } = [];
    public IEnumerable<CampaignStat> CampaignStats { get; set; } = [];
}
