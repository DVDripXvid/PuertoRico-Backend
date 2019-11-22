using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Events;
using PuertoRico.Engine.SignalR;
using PuertoRico.Engine.Test.Integration.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace PuertoRico.Engine.Test.Integration
{
    public abstract class TestBase
    {
        protected readonly TestWebApplicationFactory Factory;
        protected readonly Dictionary<string, GameHubProxy> Sessions = new Dictionary<string, GameHubProxy>();
        protected readonly IGameClient FakeClient = A.Fake<IGameClient>();

        public TestBase() {
            Factory = new TestWebApplicationFactory();
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

        protected async Task<(string, GameHubProxy, GameHubProxy, GameHubProxy)> Create3PlayerGame() {
            var session1 = await CreateSession("user1");
            var session2 = await CreateSession("user2");
            var session3 = await CreateSession("user3");
            string gameId = null;
            session2.GameCreated += ev => {
                gameId = ev.GameId;
                session2.JoinGame(ev.GameId).Wait();
                session3.JoinGame(ev.GameId).Wait();
            };
            await session1.CreateGame("3PlayerGame");
            session1.PlayerJoinedSignal.WaitOne();
            session1.PlayerJoinedSignal.WaitOne();
            Assert.NotNull(gameId);
            GameChangedEvent changedEvent = null;
            session1.GameChanged += ev => changedEvent = ev;
            await session1.StartGame(gameId);
            //session1.GameChangedSignal.WaitOne();
            Assert.NotNull(changedEvent);
            Assert.Equal(3, changedEvent.Game.Players.Count);
            Assert.Equal(3, changedEvent.Game.Players.Select(p => p.UserId).Distinct().Count());
            return (gameId, session1, session2, session3);
        }

        protected static async Task SelectRole(GameHubProxy session, string roleName) {
            var roleIdx = session.GameState.SelectableRoles.Single(r => r.Name == roleName).Index;
            await session.SelectRole(session.GameState.Id, new SelectRole { RoleIndex = roleIdx});
        }
    }
}