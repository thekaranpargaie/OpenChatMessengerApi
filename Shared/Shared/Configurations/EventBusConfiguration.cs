namespace Shared.Configurations
{
    public class EventBusConfiguration
    {
        public string Provider { get; set; }
        public RabbitMQConfiguration RabbitMQConfig { get; set; }
        public AzureServiceBusConfiguration AzureServiceBusConfig  { get; set; }
    }
    public class RabbitMQConfiguration
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string QueueName { get; set; }
        public ushort Port { get; set; }
        public string VirtualHost { get; set; }
    }
    public class AzureServiceBusConfiguration
    {
        public string ConnectionString { get; set; }
    }
}
