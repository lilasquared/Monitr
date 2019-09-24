using System;
using DotNetify;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Monitr
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSingleton<MonitrService>();
            services.AddSingleton<IObservable<StatsRecord>>(ctx => ctx.GetService<MonitrService>());
            services.AddSingleton<IHostedService, MonitrService>(ctx => ctx.GetService<MonitrService>());

            services.AddLogging();
            services.AddMemoryCache();
            services.AddSignalR();
            services.AddDotNetify();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseWebSockets();
            app.UseSignalR(routes => routes.MapDotNetifyHub());
            app.UseDotNetify();

            app.MapWhen(
                context => !context.Request.Path.Value.Contains("."),
                branch =>
                {
                    branch.Use((context, next) =>
                    {
                        context.Request.Path = new Microsoft.AspNetCore.Http.PathString("/index.html");
                        return next();
                    });
                    branch.UseStaticFiles();
                }
            );

            app.UseStaticFiles();
        }
    }
}
