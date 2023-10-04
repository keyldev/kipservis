namespace ReportService.Models
{
    public class ReportInfoResponse // DTO object
    {
        public Guid Id { get; set; }
        public int Progress { get; set; }
        public string? Result { get; set; }
    }
}
