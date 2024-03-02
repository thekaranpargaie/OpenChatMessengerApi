using Base.Contracts;
using Base.Infrastructure.EventBus;
using MassTransit;

namespace Base.EventBus
{
    public class MassTransitEventBusClient : IEventsBus
    {
        private readonly IBusControl busControl;

        public void Dispose()
        {
            throw new NotImplementedException();
        }
        public MassTransitEventBusClient(IBusControl busControl)
        {
            this.busControl = busControl;
            this.busControl.Start();
        }
        public async Task PublishAsync<T>(T @event) where T : IntegrationEvent
        {
            await this.busControl.Publish<T>(@event);
        }

        public void StartConsuming()
        {
            this.busControl.Start();
            throw new NotImplementedException();
        }

        public void Subscribe<T>(IIntegrationEventHandler<T> handler) where T : IntegrationEvent
        {
            throw new NotImplementedException();
        }
    }
}
