using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SyncService.Consumers;
using SyncService.Services;

namespace SyncService
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ISyncronisationService, SyncronisationService>();
            services.AddMassTransit(x =>
            {
                x.AddConsumer<TestReadyConsumer>();

                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    //THESE SECRETS PROBABLY WANT TO COME FROM ENV VARIABLES WHEN IN PROD

                    cfg.Host(new Uri($"rabbitmq://127.0.0.1:5672"), hostConfig =>
                    {
                        hostConfig.Username("guest");
                        hostConfig.Password("guest");
                    });

                    cfg.ReceiveEndpoint("test_ready", ep =>
                    {
                        ep.PrefetchCount = 16;
                        //ep.UseMessageRetry(r => r.Interval(2, 100));

                        ep.ConfigureConsumer<TestReadyConsumer>(provider);
                    });

                    //cfg.ReceiveEndpoint("next_action", ep =>
                    //{
                    //    ep.PrefetchCount = 16;
                    //    //ep.UseMessageRetry(r => r.Interval(2, 100));

                    //    ep.ConfigureConsumer<TestReadyConsumer>(provider);
                    //});




                }));
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });

            var bus = app.ApplicationServices.GetService<IBusControl>();

            var busHandle = TaskUtil.Await(() =>
            {
                return bus.StartAsync();
            });

            lifetime.ApplicationStopping.Register(() =>
            {
                busHandle.Stop();
            });

        }
    }
}
