using DemoConsoleApplication.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PortCMIS;
using PortCMIS.Client;
using PortCMIS.Client.Impl;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoConsoleApplication
{
    public static class Startup
    {
        public static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddSingleton<IApplication, ConsoleApplication>();
            services.Configure<CmisOptions>(context.Configuration.GetSection(nameof(CmisOptions)));

            var cmisOptions = context.Configuration.GetSection(nameof(CmisOptions)).Get<CmisOptions>();
            services.AddSingleton<ISessionFactory, SessionFactory>(_ => SessionFactory.NewInstance());
            services.AddSingleton(s =>
            {
                var factory = s.GetRequiredService<ISessionFactory>();
                return factory.CreateSession(new Dictionary<string, string>()
                                            {
                                                { SessionParameter.User, cmisOptions.User },
                                                { SessionParameter.Password, cmisOptions.Password },
                                                { SessionParameter.AtomPubUrl, cmisOptions.AtomUrl },
                                                { SessionParameter.BindingType, BindingType.AtomPub },
                                                { SessionParameter.RepositoryId, cmisOptions.RepositoryId }
                                            }
                );
            });
        }
    }
}
