//using Base.Contracts;
//using MassTransit;
//using MassTransit.AutofacIntegration;
//using Shared.Configurations;

//namespace User.Infrastructure.Configuration.EventsBus
//{
//    internal class EventsBusModule : Autofac.Module
//    {
//        private readonly IEventsBus _eventsBus;
//        private readonly EventBusConfiguration _busConfiguration;

//        public EventsBusModule(IEventsBus eventsBus, EventBusConfiguration busConfiguration)
//        {
//            _eventsBus = eventsBus;
//            _busConfiguration = busConfiguration;
//        }

//        protected override void Load(ContainerBuilder builder)
//        {
//            //TODO: Need to update once we get the azure service bus credentials
////            builder.AddMassTransit(cfg =>
////            {
////                //cfg.AddConsumer<UserManagementExportRequestConsumer>();
////                cfg.Builder.Register(context =>
////                {
////                    var busControl = GetConfiguredEventBus(context, cfg);
////                    return busControl;
////                })
////                    .SingleInstance()
////                    .As<IBusControl>()
////                    .As<IBus>();
////            });

////if (_eventsBus != null)
////            {
////                builder.RegisterInstance(_eventsBus).SingleInstance();
////            }
////            else
////            {
////                builder.RegisterType<MassTransitEventBusClient>()
////                .As<IEventsBus>()
////                .SingleInstance();
////            }

//        }
//        public IBusControl GetConfiguredEventBus(IComponentContext context, IContainerBuilderBusConfigurator cfg)
//        {
//            if(_busConfiguration.Provider == "RabbitMQ")
//            {
//                var rabbitMQConfiguration = _busConfiguration.RabbitMQConfig;
//                return Bus.Factory.CreateUsingRabbitMq(bus =>
//                {
//                    bus.Host(rabbitMQConfiguration.HostName, rabbitMQConfiguration.Port, rabbitMQConfiguration.VirtualHost, h =>
//                    {
//                        h.Username(rabbitMQConfiguration.UserName);
//                        h.Password(rabbitMQConfiguration.Password);

//                    });
//                    cfg.AddConsumersFromContainer(context);
//                    bus.ReceiveEndpoint(rabbitMQConfiguration.QueueName, ec =>
//                    {
//                        //ec.Consumer<UserManagementExportRequestConsumer>();
//                    });
//                });
//            }
//            else
//            {
//                //Code for Azure Service Bus
//                return null;
//            }
//        }
//    }


//}