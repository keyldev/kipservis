using System.ComponentModel.DataAnnotations;

namespace ReportService.Models
{
    public class ReportRequest
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; } // 
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int Progress { get; set; }
        public string? Result { get; set; } // random json response
        public DateTime CreatedAt { get; set; } // time when request created 

    }
}
