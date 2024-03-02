namespace Shared.Configurations
{
    public class AppSettings
    {
        public Boolean LoggingEnabled { get; set; }
        public ServiceConfig ServiceConfig { get; set; }
        public EventBusConfiguration EventBusConfig { get; set; }
    }
}
