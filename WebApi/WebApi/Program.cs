using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Serilog;
using Serilog.Events;

namespace WebApi
{
    public class Proram
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddJsonFile("appsettings.json", false, true);

            Log.Logger = new LoggerConfiguration()
                            .ReadFrom.Configuration(builder.Configuration)
                            .MinimumLevel.Information()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Error)
                            .MinimumLevel.Override("System", LogEventLevel.Warning)
                            .WriteTo.Console()
                            .WriteTo.File(@"/logs/apiLog.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 10)
                            .CreateLogger();
            builder.Host.UseSerilog();

            builder.Services.AddControllers();

            #region RabbitMQ
            var factory = new ConnectionFactory()
            {
                HostName = "rabbitmq",
                UserName = "guest",
                Password = "guest"
            };
            builder.Services.AddSingleton(factory.CreateConnection());
            #endregion

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
// Scaffold-DbContext "Host=db;Port=5432;Username=postgres;Password=password;Database=db_test" Npgsql.EntityFrameworkCore.PostgreSQL
// Scaffold-DbContext "Host=192.168.88.196;Port=5432;Username=postgres;Password=password;Database=db_test" Npgsql.EntityFrameworkCore.PostgreSQL