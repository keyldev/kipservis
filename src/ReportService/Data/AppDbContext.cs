using Microsoft.EntityFrameworkCore;
using ReportService.Models;

namespace ReportService.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<ReportRequest> UserStatisticsRequests { get; set; }
        public AppDbContext()
        {
            Database.EnsureCreated();
        }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

    }
}
