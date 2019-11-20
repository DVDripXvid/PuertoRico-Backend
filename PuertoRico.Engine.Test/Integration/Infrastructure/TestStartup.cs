using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PuertoRico.Engine.SignalR;

namespace PuertoRico.Engine.Test.Integration.Infrastructure
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void AddSignalR(IServiceCollection services)
        {
            services.AddSignalR();
        }

        protected override void UseSignalR(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<GameHub>("game");
            });
        }
    }
}