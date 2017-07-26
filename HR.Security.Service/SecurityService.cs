using System;
using Autofac;
using MassTransit;
using HR.Security.Core.Autofac;
using HR.Security.MassTransit.AutoFac;

namespace HR.Security.Service
{
    class SecurityService
    {
        private IContainer _container;

        public SecurityService()
        {
            // Dependency Registrer
            var builder = new ContainerBuilder();

            // Register Module.
            builder.RegisterModule<ApplicationModule>();
            builder.RegisterModule<MassTransitModule>();

            var container = builder.Build();

            _container = container;
        }

        public void Start()
        {
            _container.Resolve<IBusControl>().Start();
        }

        public void Stop()
        {
            _container.Resolve<IBusControl>().Stop();
        }
    }
}
