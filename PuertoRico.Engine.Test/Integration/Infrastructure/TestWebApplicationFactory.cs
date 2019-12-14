using FakeItEasy;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PuertoRico.Engine.Services;

namespace PuertoRico.Engine.Test.Integration.Infrastructure
{
    public class TestWebApplicationFactory : WebApplicationFactory<TestStartup>
    {
        public FakeUserService FakeUserService = new FakeUserService();

        public T GetService<T>() {
            using var scope = Services.CreateScope();
            return scope.ServiceProvider.GetService<T>();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IUserService>(FakeUserService);
            });
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder()
                .ConfigureLogging(logging => {
                    logging.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Debug);
                })
                .UseStartup<TestStartup>();
        }
    }
}