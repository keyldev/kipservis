using ReportService.Models;

namespace ReportService.Services
{
    public interface IReportService
    {

        public Task<Guid> CreateUserStatisticsRequest(UserStatisticsRequest request);
        public Task<ReportRequest> GetRequestInfo(Guid id);
        public Task ProcessReport(Guid requestId);
        public void DefaultProcessReport(Guid requestId);
    }
}
