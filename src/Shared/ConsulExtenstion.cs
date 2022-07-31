

using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Shared
{
    public static class ConsulExtenstion { 
        public static void AddConsulToApp(this IServiceCollection services,string address)
        {
            services.AddSingleton<IConsulClient, ConsulClient>(c => new ConsulClient(config =>
            {
                config.Address = new Uri(address);
            }));
        }

        public static void UseConsul(this IApplicationBuilder app, IHostApplicationLifetime lifetime)
        {
            var consulClient = app.ApplicationServices
                               .GetRequiredService<IConsulClient>();
            ConsulServiceSetting consulConfig = app.ApplicationServices
                                .GetRequiredService<IOptions<ConsulServiceSetting>>().Value;

            // Register service with consul
            var registration = new AgentServiceRegistration()
            {
                ID = consulConfig.Name,
                Name = consulConfig.Name,
                Address = consulConfig.Host,
                Port = consulConfig.Port,
                EnableTagOverride = true,
                Tags = consulConfig.Tags,
                Checks = new AgentServiceCheck[]  {
                    new AgentCheckRegistration()
                    {
                        TCP=$"{consulConfig.Host}:{consulConfig.Port}",
                        Notes = "Checks /health/status",
                        Timeout = TimeSpan.FromSeconds(3),
                        Interval = TimeSpan.FromSeconds(10)
                    }
                }
            };

            consulClient.Agent.ServiceDeregister(registration.ID).GetAwaiter().GetResult();
            consulClient.Agent.ServiceRegister(registration).GetAwaiter().GetResult();

            lifetime.ApplicationStopping.Register(() => {
                consulClient.Agent.ServiceDeregister(registration.ID).GetAwaiter().GetResult();
            });
        } 
    }
}