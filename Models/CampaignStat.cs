namespace MediaPortal.Api.Models;

public class CampaignStat
{
    public string Publisher { get; set; } = "";
    public string? AcquisitionOffer { get; set; }
    public int SubscriberCount { get; set; }
    public int EmailSubscribers { get; set; }
    public int ConversionCount { get; set; }
    public int ConvertedSubscribers { get; set; }
    public decimal ConversionRate { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal RevenuePerSubscriber { get; set; }
    public decimal? AvgDaysToFirstConversion { get; set; }
}
