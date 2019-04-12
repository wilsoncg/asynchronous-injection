using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ploeh.Samples.BookingApi;

namespace BookingApi.WebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSingleton<ActorService>(s => 
            {
                var app = ActorSystem.Create("app");
                IActorRef a = app.ActorOf<CreateReservationActor>();
                return new ActorService(app);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }

    public class ActorService : IDisposable
    {
        readonly ActorSystem _system;

        public ActorService(ActorSystem system)
        {
            _system = system;
        }

        public void Dispose()
        {
            _system.Terminate();
            _system.Dispose();
        }
    }    

    public static class ActorMiddlewareExtensions
    {
        public static IApplicationBuilder UseActors(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ActorsMiddleware>();
        }
    }

    public class ActorsMiddleware
    {
        private readonly RequestDelegate _next;

        public ActorsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
