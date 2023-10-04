using Microsoft.EntityFrameworkCore;
using ReportService.Data;
using ReportService.Repository;
using ReportService.Services;

namespace ReportService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<IReportService, Services.ReportService>(); // ругается на нейминг (проект называется также как и сервис)
            builder.Services.AddScoped<IReportRepository, ReportRepository>();

            builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version(8, 0, 32))), ServiceLifetime.Singleton); // 


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}