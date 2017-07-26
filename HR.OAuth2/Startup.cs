using System;
using System.Reflection;
using System.Web.Mvc;
using System.Linq;
using Owin;
using Microsoft.Owin;
using Autofac;
using Autofac.Integration.Mvc;
using HR.Security.Core.Autofac;
using HR.Security.Core.Services.Users;
using HR.OAuth2.Filters;

[assembly: OwinStartup(typeof(HR.OAuth2.Startup))]

namespace HR.OAuth2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Dependency Registrer
            var builder = new ContainerBuilder();

            // Register Module.
            builder.RegisterModule<ApplicationModule>();

            // Register Mvc Controller.
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            // Register Mvc Filter.
            builder.RegisterFilterProvider();

            builder.Register(x => new AuthorizationServerAuthorizeAttribute(x.Resolve<IUserAccountService>())).AsAuthorizationFilterFor<Controller>().SingleInstance();

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            // Register the Autofac middleware FIRST. This also adds
            // Autofac-injected middleware registered with the container.
            app.UseAutofacMiddleware(container);

            ConfigureAuth(app);
        }
    }
}
