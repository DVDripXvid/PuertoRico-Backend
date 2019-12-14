using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PuertoRico.Engine.DAL;
using PuertoRico.Engine.SignalR;

namespace PuertoRico.Engine.Test.Integration.Infrastructure
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void AddCosmos(IServiceCollection services) {
            var jsonSettings = new JsonSerializerSettings {
                TypeNameHandling = TypeNameHandling.Objects
            };
            var client = new CosmosClientBuilder(
                    "https://localhost:8081",
                    "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==")
                .WithConnectionModeDirect()
                .WithCustomSerializer(new NewtonsoftJsonCosmosSerializer(jsonSettings))
                .Build();

            services.AddSingleton(client);
        }

        protected override ISignalRServerBuilder AddSignalR(IServiceCollection services)
        {
            return services.AddSignalR();
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory) {
            loggerFactory.AddFile("Logs/PuertoRico-{Date}.txt");

            app.Use((ctx, next) => {
                ctx.User = new ClaimsPrincipal();
                return next();
            });
            
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<GameHub>("game");
            });
        }
    }
}