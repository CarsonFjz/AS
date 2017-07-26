using System;
using Topshelf;

namespace HR.Security.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<SecurityService>(s =>
                {
                    s.ConstructUsing(name => new SecurityService());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                x.RunAsLocalSystem();

                x.SetDescription("AuthorizationServer Service");
                x.SetDisplayName("AuthorizationServer Service");
                x.SetServiceName("AuthorizationServerService");
            });
        }
    }
}
