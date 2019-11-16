using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace PuertoRico.Engine.Test.Integration
{
    public class TestWebApplicationFactory : WebApplicationFactory<TestStartup>
    {
        protected override IWebHostBuilder CreateWebHostBuilder() {
            return WebHost.CreateDefaultBuilder().UseStartup<TestStartup>();
        }
    }
}