using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain;
using Xunit;

namespace PuertoRico.Engine.Test.Integration
{
    public class SampleTest : IClassFixture<TestWebApplicationFactory>
    {
        private readonly WebApplicationFactory<TestStartup> _factory;
        private readonly HubConnection _gameHub;

        public SampleTest(TestWebApplicationFactory factory) {
            _factory = factory;
            _gameHub = new HubConnectionBuilder()
                .WithUrl(new Uri(_factory.Server.BaseAddress, "game"),
                    o => { o.HttpMessageHandlerFactory = _ => _factory.Server.CreateHandler(); }).Build();
        }

        [Fact]
        public async Task Test() {
            _gameHub.On<Game>("gameStateChanged", game => { });
            await _gameHub.StartAsync();
            await _gameHub.InvokeAsync("ExecuteAction", "asd", new Build {
                BuildingIndex = 5,
            });
        }
    }
}