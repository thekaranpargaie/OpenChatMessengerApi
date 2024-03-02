namespace User.Infrastructure.Configuration.EventsBus
{
    public static class EventsBusStartup
    {
        public static void Initialize(
          )
        {
            SubscribeToIntegrationEvents();
        }

        private static void SubscribeToIntegrationEvents()
        {
            //var eventBus = UserCompositionRoot.BeginLifetimeScope().Resolve<IEventsBus>();
       }
    }
}