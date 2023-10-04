using Microsoft.EntityFrameworkCore;
using ReportService.Data;
using ReportService.Models;
using System.Diagnostics;

namespace ReportService.Repository
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _dbContext;
        public ReportRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddUserStatisticsRequestAsync(ReportRequest request)
        {
            await _dbContext.UserStatisticsRequests.AddAsync(request);
            await _dbContext.SaveChangesAsync();

        }

        public async Task<ReportRequest> GetRequestInfo(Guid id)
        {
            return await _dbContext.UserStatisticsRequests.FindAsync(id);
        }


        public async Task SaveRequestAsync(ReportRequest reportRequest)
        {
            try
            {
                var request = await _dbContext.UserStatisticsRequests.FindAsync(reportRequest.Id);
                request.Progress = reportRequest.Progress;
                request.Result = reportRequest.Result;


                _dbContext.UserStatisticsRequests.Update(request);
                await _dbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception " + ex.ToString());
            }
        }
    }
}
