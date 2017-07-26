using Autofac;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR.Security.Core.Data;
using HR.Security.Core.Services.Users;
using HR.Security.Core.Services.Menu;
using HR.Security.Core.Settings;
using HR.Security.Core.Services.Clients;

namespace HR.Security.Core.Autofac
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register<DbConnection>(x =>
            {
                var connectionString = AppSettingConfig.ASConnectionString;

                var connection = new SqlConnection(connectionString);

                connection.Open();

                return connection;
            })
            .AsSelf()
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
            
            builder.RegisterType<SecurityObjectContext>().AsSelf().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();

            //services
            builder.RegisterType<MenuService>().As<IMenuService>();
            builder.RegisterType<ClientService>().As<IClientService>();
            builder.RegisterType<UserAccountService>().As<IUserAccountService>();
        }
    }
}
