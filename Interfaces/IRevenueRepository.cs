using MediaPortal.Api.Models;

namespace MediaPortal.Api.Interfaces;

public interface IRevenueRepository
{
    Task<IEnumerable<RevenueDto>> GetRevenueByPublisherAsync(string publisher, DateOnly startDate, DateOnly endDate);
}
