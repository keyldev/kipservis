using ReportService.Models;

namespace ReportService.Repository
{
    public interface IReportRepository
    {
        public Task<Guid> AddUserStatisticsRequestAsync(ReportRequest request);
        public Task<ReportRequest> GetRequestInfo(Guid id);
        Task SaveRequestAsync(ReportRequest reportRequest);

    }
}
