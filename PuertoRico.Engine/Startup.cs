using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PuertoRico.Engine.DAL;
using PuertoRico.Engine.DAL.Redis;
using PuertoRico.Engine.Services;
using PuertoRico.Engine.SignalR.Hubs;
using PuertoRico.Engine.Stores;
using PuertoRico.Engine.Stores.InMemory;
using StackExchange.Redis;

namespace PuertoRico.Engine
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
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            };

            services.AddMvc();

            services.AddApplicationInsightsTelemetry();
            services.AddHttpContextAccessor();
            services.AddCors(options => options
                .AddDefaultPolicy(builder => builder
                    .AllowCredentials()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithOrigins(
                        "http://localhost:3000"
                    )
                ));

            AddSignalR(services)
                .AddJsonProtocol(c => c.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            services.AddSingleton(ConnectionMultiplexer.Connect(Configuration["REDIS_URL"]));

            services.AddTransient<IGameService, GameService>();
            services.AddTransient<IReplayableGameService, ReplayableGameService>();
            services.AddSingleton<IInProgressGameStore, InMemoryInProgressGameStore>();
            services.AddSingleton<IUserService, AuthenticatedUserService>();

            services.AddSingleton<IGameRepository, RedisGameRepository>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(jwt =>
                {
                    jwt.Audience = "178792157062-b5jtd265enrjn20s04hqtfntm9esshrf.apps.googleusercontent.com";
                    jwt.Authority = "https://accounts.google.com";
                    jwt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuers = new[] { "https://accounts.google.com", "accounts.google.com" },
                        ValidateAudience = true,
                        ValidAudience = "178792157062-b5jtd265enrjn20s04hqtfntm9esshrf.apps.googleusercontent.com",
                    };
                    jwt.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = ctx =>
                        {
                            var accessToken = ctx.Request.Query["access_token"];
                            ctx.Token = accessToken;
                            return Task.CompletedTask;
                        }
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors();
            app.UseAuthentication();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<LobbyHub>("lobby");
                endpoints.MapHub<GameHub>("game");
            });

            // trigger seeding the in memory store from db
            using var scope = app.ApplicationServices.CreateScope();
            scope.ServiceProvider.GetService<IInProgressGameStore>();
        }

        protected virtual ISignalRServerBuilder AddSignalR(IServiceCollection services)
        {
            return services.AddSignalR()
                /*.AddAzureSignalR(opts => {
                    opts.ServerStickyMode = ServerStickyMode.Required;
                    opts.ConnectionCount = 2;
                })*/;
        }
    }
}