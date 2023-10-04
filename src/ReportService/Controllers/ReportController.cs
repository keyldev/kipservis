using Microsoft.AspNetCore.Mvc;
using ReportService.Models;
using ReportService.Services;

namespace ReportService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IConfiguration _configuration;
        private readonly int _reportTimeout;

        public ReportController(IConfiguration configuration, IReportService reportService)
        {
            _reportService = reportService;
            _configuration = configuration;
            _reportTimeout = _configuration.GetValue<int>("ReportTimeout", 60000);
        }

        [HttpPost("user_statistics")]
        public async Task<IActionResult> CreateUserStatisticsRequest([FromBody] UserStatisticsRequest request)
        {
            var requestId = await _reportService.CreateUserStatisticsRequest(request);

            //
            //_reportService.ProcessReport(requestId); // если каждые 25% сохранять результат
            _reportService.DefaultProcessReport(requestId); // если просто подождать X секунд, и сохранить значение на сотке.

            return Ok(new
            {
                requestId = requestId
            });
        }

        [HttpGet("info/{id}")]
        public async Task<IActionResult> GetRequestInfo(Guid id)
        {
            var requestInfo = await _reportService.GetRequestInfo(id);

            if (requestInfo is null) return NotFound();

            var progress = (int)((DateTime.UtcNow - requestInfo.CreatedAt).TotalMilliseconds / _reportTimeout * 100);
            if (progress > 100)
            {
                requestInfo = await _reportService.GetRequestInfo(id);
                progress = 100;
            }
            var response = new ReportInfoResponse
            {
                Id = requestInfo.Id,
                Progress = progress,
                Result = requestInfo.Result,
            };

            return Ok(response);

            /*
            #region ЕСЛИ_ИСПОЛЬЗУЕТСЯ_МЕТОД_PROCESSREPORT
            if (requestInfo is not null)
            {
                if(requestInfo.Progress < 100)
                {
                    _reportService.ProcessReport(id);
                    requestInfo = await _reportService.GetRequestInfo(id);
                }

                var response = new ReportInfoResponse
                {
                    Id = requestInfo.Id,
                    Progress = requestInfo.Progress,
                    Result = requestInfo.Result,
                };
                return Ok(response);
            }
            return NotFound();
            #endregion*/

        }
    }
}
