using System;
using Base.Infrastructure.EventBus;

namespace Base.Contracts
{
    public interface IEventsBus : IDisposable
    {
        Task PublishAsync<T>(T @event) where T : IntegrationEvent;

        void Subscribe<T>(IIntegrationEventHandler<T> handler) where T : IntegrationEvent;

        void StartConsuming();
        
    }
}