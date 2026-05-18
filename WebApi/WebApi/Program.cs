using RabbitMQ.Client;
using Serilog;
using Serilog.Events;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace WebApi
{
    public class Proram
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddJsonFile("appsettings.json", false, true);

            #region OpenTelemetry
            var serviceName = Environment.GetEnvironmentVariable("OTEL_SERVICE_NAME")
                              ?? builder.Configuration["Service:Name"]
                              ?? builder.Environment.ApplicationName;
            var resourceBuilder = ResourceBuilder.CreateDefault().AddService(serviceName);
            #endregion

            #region Logger Serilog
            Log.Logger = new LoggerConfiguration()
                            .ReadFrom.Configuration(builder.Configuration)
                            .MinimumLevel.Information()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Error)
                            .MinimumLevel.Override("System", LogEventLevel.Warning)
                            .WriteTo.Console()
                            // .WriteTo.File(@"/logs/apiLog.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 10)
                            .CreateLogger();
            builder.Host.UseSerilog();
            #endregion

            builder.Services.AddControllers();
            builder.Services.AddHttpClient();

            builder.Services.AddHttpClient("MyApiClient", client =>
            {
                client.BaseAddress = new Uri("https://cbr.ru/scripts/XML_daily.asp?date_req=17/05/2026");
            });

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

            #region OpenTelemetry
            builder.Services.AddOpenTelemetry()
                .WithTracing(tracerProviderBuilder => tracerProviderBuilder
                    .SetResourceBuilder(resourceBuilder)
                    .AddSource("WebApi")
                    .AddAspNetCoreInstrumentation(options => options.RecordException = true)
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddOtlpExporter())
                .WithMetrics(meterProviderBuilder => meterProviderBuilder
                    .SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter());

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddOpenTelemetry(options =>
            {
                options.SetResourceBuilder(resourceBuilder);
                options.IncludeFormattedMessage = true;
                options.IncludeScopes = true;
                options.ParseStateValues = true;
                options.AddOtlpExporter();
            });
            #endregion

            #region Проверим значения environmentVariables (...+launchSettings.json) 
            var app = builder.Build();

            foreach (var c in builder.Configuration.AsEnumerable())
            {
                Console.WriteLine(c.Key + " = " + c.Value);
            }
            #endregion

            #region Development
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                // app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
            }
            #endregion

            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
#region Console Nuget / EntityFrameworkCore.PostgreSQL (DataBaseFirst)
//// Home: etc/hosts 192.168.88.196 (db, postgres, rabbitmq)
// Scaffold-DbContext "Host=postgres;Port=5432;Username=postgres;Password=password;Database=db_test" Npgsql.EntityFrameworkCore.PostgreSQL
#endregion