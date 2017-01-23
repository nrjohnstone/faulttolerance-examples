using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using WebApi.Policies;
using WebApi.Proxies;
using WebApi.Repositories;

namespace WebApi
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSingleton<IProductService, ProductServiceProxy>();
            services.AddSingleton<IOrderRepository, OrderRepository>();
            services.AddSingleton<IUserRepository, UserRepository>();

            services.AddSingleton<PolicyRegistry>();

            var policyRegistry = services.BuildServiceProvider().GetService<PolicyRegistry>();

            var databaseCircuitBreaker =
                Policy.Handle<DatabaseTimeoutException>()
                    .CircuitBreaker(exceptionsAllowedBeforeBreaking: 2, durationOfBreak: TimeSpan.FromMinutes(1),
                        onBreak: (ex, t) =>
                        {
                            Console.WriteLine("Database CB Break");
                        },
                        onReset: () =>
                        {
                            Console.WriteLine("Database CB Reset");
                        })
                    .WithPolicyKey("Database");
            
            policyRegistry.Add(databaseCircuitBreaker);

            services.AddSingleton(policyRegistry);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();            
        }
    }
}
