using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using MediaPortal.Api.Models;

namespace MediaPortal.Api.Services;

public class DashboardService
{
    private readonly string _connectionString;

    public DashboardService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException("ConnectionStrings:SqlServer is not configured.");
    }

    // GET /api/data — fetches media buy and conversions in parallel from separate SPs
    public async Task<DashboardPayload> GetDashboardPayloadAsync(
        DateOnly from, DateOnly to, bool includeConversions)
    {
        var fromDt = from.ToDateTime(TimeOnly.MinValue);
        var toDt   = to.ToDateTime(TimeOnly.MinValue);

        var mediaBuyTask      = GetMediaBuyAsync(fromDt, toDt);
        var conversionsTask   = includeConversions
            ? GetConversionsAsync(fromDt, toDt)
            : Task.FromResult(new List<ConversionRecord>());
        var campaignStatsTask = GetCampaignStatsAsync(fromDt, toDt);

        await Task.WhenAll(mediaBuyTask, conversionsTask, campaignStatsTask);

        return new DashboardPayload
        {
            MediaBuy      = mediaBuyTask.Result,
            Conversions   = conversionsTask.Result,
            CampaignStats = campaignStatsTask.Result,
        };
    }

    // POST /api/upload — upserts media buy rows + inserts conversions
    public async Task<UploadResponse> UploadAsync(UploadRequest request)
    {
        var response = new UploadResponse();

        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        // Media buy — MERGE via sp_UpsertMediaBuy
        foreach (var row in request.MediaBuy)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@SubscriberID",           row.SubscriberId);
                p.Add("@MonthStart",             row.MonthStart,              DbType.Date);
                p.Add("@Publisher",              row.Publisher);
                p.Add("@SourceName",             row.SourceName);
                p.Add("@AcquisitionOffer",       row.AcquisitionOffer);
                p.Add("@LeadChannel",            row.LeadChannel);
                p.Add("@SubscriptionCreateDate", row.SubscriptionCreateDate,  DbType.DateTime);

                var result = await conn.QuerySingleAsync<int>(
                    "dbo.sp_UpsertMediaBuy",
                    p,
                    commandType: CommandType.StoredProcedure);

                // sp returns 1 = inserted, 2 = updated
                if (result == 1) response.Inserted++;
                else             response.Updated++;
            }
            catch (Exception ex)
            {
                response.Errors.Add($"MediaBuy SubscriberID={row.SubscriberId}: {ex.Message}");
            }
        }

        // Conversions — INSERT via sp_InsertConversion
        foreach (var row in request.Conversions)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@Placement",              row.Placement);
                p.Add("@SourceName",             row.SourceName);
                p.Add("@SubscriptionCreateDate", row.SubscriptionCreateDate, DbType.Date);
                p.Add("@ConversionDate",         row.ConversionDate,         DbType.Date);
                p.Add("@Amount",                 row.Amount,                 DbType.Decimal);
                p.Add("@OfferName",              row.OfferName);
                p.Add("@SubscriberID",           row.SubscriberId,           DbType.Int32);

                await conn.ExecuteScalarAsync<int>(
                    "dbo.sp_InsertConversion",
                    p,
                    commandType: CommandType.StoredProcedure);

                response.Inserted++;
            }
            catch (Exception ex)
            {
                response.Errors.Add($"Conversion SubscriberID={row.SubscriberId}: {ex.Message}");
            }
        }

        return response;
    }

    private async Task<List<MediaBuyRecord>> GetMediaBuyAsync(DateTime from, DateTime to)
    {
        using var conn = new SqlConnection(_connectionString);

        var parameters = new DynamicParameters();
        parameters.Add("@FromDate", from, DbType.Date);
        parameters.Add("@ToDate",   to,   DbType.Date);

        var result = await conn.QueryAsync<MediaBuyRecord>(
            "dbo.sp_GetMediaBuyData",
            parameters,
            commandType: CommandType.StoredProcedure,
            commandTimeout: 60);

        return result.ToList();
    }

    private async Task<List<ConversionRecord>> GetConversionsAsync(DateTime from, DateTime to)
    {
        using var conn = new SqlConnection(_connectionString);

        var parameters = new DynamicParameters();
        parameters.Add("@FromDate", from.Date, DbType.Date);
        parameters.Add("@ToDate",   to.Date,   DbType.Date);

        var result = await conn.QueryAsync<ConversionRecord>(
            "dbo.sp_GetConversionData",
            parameters,
            commandType: CommandType.StoredProcedure,
            commandTimeout: 60);

        return result.ToList();
    }

    private async Task<List<CampaignStat>> GetCampaignStatsAsync(DateTime from, DateTime to)
    {
        using var conn = new SqlConnection(_connectionString);

        var parameters = new DynamicParameters();
        parameters.Add("@FromDate", from, DbType.Date);
        parameters.Add("@ToDate",   to,   DbType.Date);

        var result = await conn.QueryAsync<CampaignStat>(
            "dbo.sp_GetCampaignStats",
            parameters,
            commandType: CommandType.StoredProcedure,
            commandTimeout: 60);

        return result.ToList();
    }
}
