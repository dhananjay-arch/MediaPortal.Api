namespace MediaPortal.Api.Models;

public class RevenueDto
{
    public string  SourceName  { get; set; } = "";
    public decimal SmsRev      { get; set; }
    public decimal EmailRev    { get; set; }
    public decimal OtherRev    { get; set; }
    public decimal TotalRev    { get; set; }
    public int     RecordCount { get; set; }
}
