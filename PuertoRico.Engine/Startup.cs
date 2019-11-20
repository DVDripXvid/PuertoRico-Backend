using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PuertoRico.Engine.Services;
using PuertoRico.Engine.SignalR;
using PuertoRico.Engine.Stores;
using PuertoRico.Engine.Stores.InMemory;

namespace PuertoRico.Engine
{
    public class Startup
    {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddMvc();
            services.AddHttpContextAccessor();

            services.AddTransient<IGameService, GameService>();
            services.AddSingleton<IGameStore, InMemoryGameStore>();
            services.AddSingleton<IUserService, UserService>();

            AddSignalR(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            UseSignalR(app);
        }

        protected virtual void AddSignalR(IServiceCollection services) {
            services.AddSignalR()
                .AddAzureSignalR();
        }

        protected virtual void UseSignalR(IApplicationBuilder app) {
            app.UseAzureSignalR(routes => { routes.MapHub<GameHub>("/game"); });
        }
    }
}