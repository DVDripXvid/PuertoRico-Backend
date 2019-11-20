using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using PuertoRico.Engine.Test.Integration.Infrastructure;
using Xunit;

namespace PuertoRico.Engine.Test.Integration
{
    public abstract class TestBase : IClassFixture<TestWebApplicationFactory>
    {
        protected readonly TestWebApplicationFactory Factory;
        protected readonly Dictionary<string, GameHubProxy> Sessions = new Dictionary<string, GameHubProxy>();
        
        
        public TestBase(TestWebApplicationFactory factory) {
            Factory = factory;
        }

        protected async Task<GameHubProxy> CreateSession(string userId) {
            var hub = new HubConnectionBuilder()
                .WithUrl(new Uri(Factory.Server.BaseAddress, "game"),
                    o => { o.HttpMessageHandlerFactory = _ => Factory.Server.CreateHandler(); }).Build();
            await hub.StartAsync();
            var proxy = new GameHubProxy(hub, Factory, userId);
            Sessions.Add(userId, proxy);
            return proxy;
        }
    }
}