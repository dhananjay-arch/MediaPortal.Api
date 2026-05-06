using System.Data;
using Dapper;
using MediaPortal.Api.Interfaces;
using MediaPortal.Api.Models;
using Microsoft.Data.SqlClient;

namespace MediaPortal.Api.Repositories;

public class RevenueRepository : IRevenueRepository
{
    private readonly string _connectionString;

    public RevenueRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException("Connection string 'SqlServer' is not configured.");
    }

    public async Task<IEnumerable<RevenueDto>> GetRevenueByPublisherAsync(string publisher, DateOnly startDate, DateOnly endDate)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Publisher", publisher,             DbType.String);
        parameters.Add("@StartDate", startDate.ToDateTime(TimeOnly.MinValue), DbType.Date);
        parameters.Add("@EndDate",   endDate.ToDateTime(TimeOnly.MinValue),   DbType.Date);

        using var conn = new SqlConnection(_connectionString);

        return await conn.QueryAsync<RevenueDto>(
            "usp_GetRevenueByPublisher",
            parameters,
            commandType: CommandType.StoredProcedure,
            commandTimeout: 60);
    }
}
