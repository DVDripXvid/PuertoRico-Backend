using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PuertoRico.Engine.SignalR.Hubs;

namespace PuertoRico.Engine.Test.Integration.Infrastructure
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override ISignalRServerBuilder AddSignalR(IServiceCollection services)
        {
            return services.AddSignalR();
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("Logs/PuertoRico-{Date}.txt");

            app.Use((ctx, next) =>
            {
                ctx.User = new ClaimsPrincipal();
                return next();
            });
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<GameHub>("game");
            });
        }
    }
}