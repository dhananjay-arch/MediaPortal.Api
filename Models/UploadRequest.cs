namespace MediaPortal.Api.Models;

public class UploadRequest
{
    public List<MediaBuyUploadRow> MediaBuy { get; set; } = [];
    public List<ConversionUploadRow> Conversions { get; set; } = [];
}

public class MediaBuyUploadRow
{
    public int SubscriberId { get; set; }
    public string MonthStart { get; set; } = "";           // YYYY-MM-DD
    public string Publisher { get; set; } = "";
    public string? SourceName { get; set; }
    public string? AcquisitionOffer { get; set; }
    public string LeadChannel { get; set; } = "Email";    // Email | SMS
    public string SubscriptionCreateDate { get; set; } = "";
}

public class ConversionUploadRow
{
    public int? SubscriberId { get; set; }
    public string Placement { get; set; } = "";
    public string SourceName { get; set; } = "";
    public string SubscriptionCreateDate { get; set; } = "";
    public string ConversionDate { get; set; } = "";
    public decimal Amount { get; set; }
    public string? OfferName { get; set; }
}
