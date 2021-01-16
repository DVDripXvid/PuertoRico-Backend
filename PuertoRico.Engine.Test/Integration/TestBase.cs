using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.SignalR.Client;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.DTOs;
using PuertoRico.Engine.SignalR.Commands;
using PuertoRico.Engine.SignalR.Events;
using PuertoRico.Engine.SignalR.Hubs;
using PuertoRico.Engine.Test.Integration.Infrastructure;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace PuertoRico.Engine.Test.Integration
{
    public abstract class TestBase : IDisposable
    {
        protected readonly TestWebApplicationFactory Factory;
        protected readonly Dictionary<string, GameHubProxy> Sessions = new Dictionary<string, GameHubProxy>();
        protected readonly IGameClient FakeClient = A.Fake<IGameClient>();

        public TestBase()
        {
            Factory = new TestWebApplicationFactory();
        }

        protected async Task<GameHubProxy> CreateSession(string userId)
        {
            var hub = new HubConnectionBuilder()
                .WithUrl(new Uri(Factory.Server.BaseAddress, "game"),
                    o => { o.HttpMessageHandlerFactory = _ => Factory.Server.CreateHandler(); }).Build();
            await hub.StartAsync();
            var proxy = new GameHubProxy(hub, Factory, userId);
            Sessions.Add(userId, proxy);
            return proxy;
        }

        protected async Task<(string, GameHubProxy, GameHubProxy, GameHubProxy)> Create3PlayerGame()
        {
            var session1 = await CreateSession("user1");
            var session2 = await CreateSession("user2");
            var session3 = await CreateSession("user3");
            string gameId = null;
            session2.GameCreated += ev =>
            {
                gameId = ev.GameId;
                var joinCmd = new GenericGameCmd { GameId = ev.GameId };
                session2.JoinGame(joinCmd).Wait();
                session3.JoinGame(joinCmd).Wait();
            };
            await session1.CreateGame(new CreateGameCmd { Name = "3PlayerGame" });
            session1.PlayerJoinedSignal.WaitOne(5000);
            session1.PlayerJoinedSignal.WaitOne(5000);
            Assert.NotNull(gameId);
            GameChangedEvent changedEvent = null;
            session1.GameChanged += ev => changedEvent = ev;
            await session1.StartGame(new GenericGameCmd
            {
                GameId = gameId
            });
            Assert.NotNull(changedEvent);
            Assert.Equal(3, changedEvent.Game.Players.Count);
            Assert.Equal(3, changedEvent.Game.Players.Select(p => p.UserId).Distinct().Count());
            return (gameId, session1, session2, session3);
        }

        protected GameHubProxy GetCurrentSession(GameDto game)
        {
            return Sessions[game.CurrentPlayer.UserId];
        }

        protected static async Task SelectRole(GameHubProxy session, string roleName)
        {
            var roleIdx = session.GameState.SelectableRoles.Single(r => r.Name == roleName).Index;
            await session.SelectRole(new GameCommand<SelectRole>
            {
                Action = new SelectRole { RoleIndex = roleIdx },
                GameId = session.GameState.Id
            });
        }

        public void Dispose()
        {
            Factory?.Dispose();
        }
    }
}