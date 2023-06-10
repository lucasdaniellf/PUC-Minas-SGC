using AplicacaoGerenciamentoLoja.HostedServices;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Formatting.Json;

namespace AplicacaoGerenciamentoLoja.Extensions
{
    public static class LoggingExtension
    {
        public static IHostBuilder UseApplicationLogging(this IHostBuilder host)
        {
            var isHostedService = Matching.FromSource<BaseConsumer>();


            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning).MinimumLevel.Override("System", LogEventLevel.Warning)
                .WriteTo.Logger(lg =>
                                    lg.Filter.ByIncludingOnly(isHostedService)
                                      .WriteTo.Console(outputTemplate: LoggerConfigurations.QueueConsumerLoggerTemplate))
                .WriteTo.Logger(lg =>
                                    lg.Filter.ByExcluding(isHostedService)
                                      .WriteTo.Console(outputTemplate: LoggerConfigurations.GeneralLoggerTemplate, restrictedToMinimumLevel: LogEventLevel.Information))
                .WriteTo.File(formatter: new JsonFormatter(), path: Path.GetFullPath(".") + LoggerConfigurations.FileLogLocation, rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Information)
                .Enrich.FromLogContext()
                .CreateLogger();

            host.UseSerilog();
            return host;
        }


    }

    public class LoggerConfigurations
    {
        //Templates
        public const string GeneralLoggerTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] - CorrelationId: {CorrelationId} - Usuario: {Usuario} - {Message:lj} {NewLine} {Exception}";
        public const string QueueConsumerLoggerTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] - HostedService - {Message:lj} {NewLine} {Exception}";

        //Location Development
        //public const string FileLogLocation = ".\\Logs\\logs-.json";
        //public const string FileErrorLogLocation = ".\\Logs\\logs-error-.json";

        //Location Production
        public const string FileLogLocation = "/Aplicacao/Logs/logs-.json";
        public const string FileErrorLogLocation = "/Aplicacao/Logs/logs-error-.json";

    }
}
