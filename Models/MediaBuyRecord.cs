namespace MediaPortal.Api.Models;

public class MediaBuyRecord
{
    public string MonthName { get; set; } = "";
    public int MonthNum { get; set; }
    public string MonthStart { get; set; } = "";
    public string MonthEnd { get; set; } = "";
    public string Publisher { get; set; } = "";
    public string? SourceName { get; set; }
    public string? AcquisitionOffer { get; set; }
    public int EmailLeads { get; set; }
    public int SmsLeads { get; set; }
    public int TotalLeads { get; set; }
    public string? EarliestSubscriptionDate { get; set; }
}
