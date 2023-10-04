namespace ReportService.Models
{
    public class UserStatisticsRequest
    {
        public Guid UserId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
