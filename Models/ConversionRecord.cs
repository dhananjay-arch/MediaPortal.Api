namespace MediaPortal.Api.Models;

public class ConversionRecord
{
    public int? SubscriberId { get; set; }
    public string Placement { get; set; } = "";
    public string SourceName { get; set; } = "";
    public string? AcquisitionOffer { get; set; }
    public string SubscriptionCreateDate { get; set; } = "";
    public string ConversionDate { get; set; } = "";
    public decimal Amount { get; set; }
    public string? OfferName { get; set; }
}
