using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using System.Runtime.CompilerServices;

namespace Base.Logging
{
    public interface IDebugLogger
    {
        void LogInformation(string message, [CallerMemberName] string caller = null);
        void Initialize(bool loggingEnabled);
    }

    public class DebugLogger : IDebugLogger
    {
        private readonly ILogger _logger;
        private Boolean _loggingEnabled;
        public DebugLogger(ILogger logger)
        {
            _logger = logger;
        }
        public void Initialize(bool loggingEnabled)
        {
            _loggingEnabled = loggingEnabled;
        }

        public void LogInformation(string message, [CallerMemberName] string caller = null)
        {
            try
            {
                if(_loggingEnabled)
                {
                    _logger.Information($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} [{caller}] - {message}");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
   public static class LoggingConfigurator
        {
            public static ILogger ConfigureLogger(IConfiguration configuration)
            {
                var logFile = configuration["LogFile"];
                var logLevel = configuration["LogLevel"];

                var logger = new LoggerConfiguration()
                    .WriteTo.File(logFile, restrictedToMinimumLevel: ParseLogLevel(logLevel))
                    .CreateLogger();

                return logger;
            }

            private static LogEventLevel ParseLogLevel(string logLevel)
            {
                return Enum.TryParse(logLevel, out LogEventLevel result) ? result : LogEventLevel.Information;
            }
        }

}
