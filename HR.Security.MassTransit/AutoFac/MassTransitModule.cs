using System;
using Autofac;
using MassTransit;
using HR.Security.MassTransit.Settings;
using HR.Security.MassTransit.Consumer;

namespace HR.Security.MassTransit.AutoFac
{
    public class MassTransitModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AddUserAccountCommandConsumer>();

            builder.Register(context =>
            {                                
                var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var uri = AppSettingConfig.RabbitMqBaseUri;

                    var host = cfg.Host(new Uri(uri), x =>
                    {
                        x.Username(AppSettingConfig.RabbitMqUserName);
                        x.Password(AppSettingConfig.RabbitMqUserPassword);                        
                    });

                    cfg.ReceiveEndpoint(host, AppSettingConfig.BaseQueueName + "_service", endPoint =>
                    {
                        endPoint.LoadFrom(context);
                    });

                });
                
                return busControl;
            })
            .SingleInstance()
            .As<IBusControl>()
            .As<IBus>();
        }
    }    
}
