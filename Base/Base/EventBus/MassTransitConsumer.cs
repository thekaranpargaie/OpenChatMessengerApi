using Base.Infrastructure.EventBus;
using MassTransit;

namespace Base.EventBus
{
    public interface IMassTransitConsumer<in T> : IConsumer<T> where T: IntegrationEvent 
    {
    }
}
