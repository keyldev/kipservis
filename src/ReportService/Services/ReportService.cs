using Microsoft.EntityFrameworkCore;
using ReportService.Data;
using ReportService.Models;
using ReportService.Repository;
using System.Diagnostics;

namespace ReportService.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly int _requestTimeout;
        // таймспан связан с 25% таймером
        private readonly TimeSpan _processingTimeLimit;

        public ReportService(IConfiguration config, IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
            _requestTimeout = config.GetValue<int>("ReportTimeout", 60000);


            _processingTimeLimit = TimeSpan.FromMilliseconds(_requestTimeout);
        }
        public async Task<Guid> CreateUserStatisticsRequest(UserStatisticsRequest request)
        {
            var reportRequest = new ReportRequest
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                CreatedAt = DateTime.UtcNow
            };


            var id = await _reportRepository.AddUserStatisticsRequestAsync(reportRequest);
            return id;

        }

        /// <summary>
        /// Запускает процесс обработки запроса
        /// </summary>
        /// <param name="requestId">Id запроса</param>
        public async void ProcessReport(Guid requestId)
        {
            //await Task.Delay(_requestTimeout); // Здесь должна быть какая-то обработка запроса

            var reportRequest = await _reportRepository.GetRequestInfo(requestId);
            if (reportRequest is not null)
            {
                Debug.WriteLine($"Processing time limit {_processingTimeLimit}");

                // если у нас упало приложение, и прогресс не сотка, тогда мы обнуляем всё по запросу и ставим его в очередь (если я правильно понял задание)
                if (reportRequest.Progress < 100 && (DateTime.UtcNow - reportRequest.CreatedAt).TotalSeconds > _requestTimeout / 1000)
                {
                    //reportRequest.Progress = 0; // 
                    reportRequest.CreatedAt = DateTime.UtcNow;
                }
                int progress = reportRequest.Progress;
                while (progress < 100)
                {
                    progress = (int)((DateTime.UtcNow - reportRequest.CreatedAt).TotalMilliseconds / _requestTimeout * 100); // прогресс конвертируемый в проценты
                    if (progress % 25 == 0) // каждые 25% записываем в БД
                    {
                        reportRequest.Progress = progress;
                        await _reportRepository.SaveRequestAsync(reportRequest);
                    }
                    await Task.Delay(_requestTimeout / 1000); // 
                }

                reportRequest.Result = "{\"user_id\": \"b28d0ced-8af5-4c94-8650-c7946241fd1a\", \"count_sign_in\": \"12\"}"; // когда всё ок, добавляем "result" 
                // result может быть сериализованной строкой
                await _reportRepository.SaveRequestAsync(reportRequest);
            }

        }
        // дефолтный метод с делеем на X секунд, манипулирует с базой лишь когда проходит опр. время.
        public async void DefaultProcessReport(Guid id)
        {
            await Task.Delay(_requestTimeout); // тут логика обработки запроса в 60к мс.

            var reportRequest = await _reportRepository.GetRequestInfo(id);
            if (reportRequest is not null)
            {
                reportRequest.Progress = 100;
                reportRequest.Result = "{\"user_id\": \"b28d0ced-8af5-4c94-8650-c7946241fd1a\", \"count_sign_in\": \"12\"}"; // любая сериализованная строка вместо этой.
                                                                                                                             // или можно модифицировать модель так, чтобы была не строка а базовый класс информации
                await _reportRepository.SaveRequestAsync(reportRequest); // отправляем на сохранение.
            }

        }

        public async Task<ReportRequest> GetRequestInfo(Guid id)
        {
            var requestInfo = await _reportRepository.GetRequestInfo(id);
            return requestInfo;
        }
    }
}
