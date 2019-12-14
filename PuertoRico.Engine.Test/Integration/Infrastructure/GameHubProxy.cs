using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.DTOs;
using PuertoRico.Engine.SignalR;
using PuertoRico.Engine.SignalR.Commands;
using PuertoRico.Engine.SignalR.Events;

namespace PuertoRico.Engine.Test.Integration.Infrastructure
{
    public class GameHubProxy : IGameHub
    {
        public event Action<GameCreatedEvent> GameCreated;
        public readonly AutoResetEvent GameCreatedSignal = new AutoResetEvent(false);

        public event Action<GameChangedEvent> GameChanged;
        private readonly AutoResetEvent _gameChangedSignal = new AutoResetEvent(false);

        public event Action<GameEndedEvent> GameEnded;
        public readonly AutoResetEvent GameEndedSignal = new AutoResetEvent(false);

        public event Action<GameStartedEvent> GameStarted;
        public readonly AutoResetEvent GameStartedSignal = new AutoResetEvent(false);

        public event Action<PlayerJoinedEvent> PlayerJoined;
        public readonly AutoResetEvent PlayerJoinedSignal = new AutoResetEvent(false);

        public event Action<PlayerLeftEvent> PlayerLeft;
        public readonly AutoResetEvent PlayerLeftSignal = new AutoResetEvent(false);

        public readonly Stack<IGameEvent> EventStack = new Stack<IGameEvent>();
        public volatile GameDto GameState;
        public volatile string UserId;

        private readonly HubConnection _hub;
        private readonly TestWebApplicationFactory _factory;
        private static object syncRoot = new object();

        public GameHubProxy(HubConnection hub, TestWebApplicationFactory factory, string userId) {
            _hub = hub;
            _factory = factory;
            UserId = userId;


            GameCreated += EventStack.Push;
            GameChanged += EventStack.Push;
            GameEnded += EventStack.Push;
            GameStarted += EventStack.Push;
            PlayerJoined += EventStack.Push;
            PlayerLeft += EventStack.Push;

            GameChanged += ev => GameState = ev.Game;

            hub.On<GameCreatedEvent>("GameCreated", ev => {
                GameCreated(ev);
                GameCreatedSignal.Set();
            });
            hub.On<GameChangedEvent>("GameChanged", ev => {
                GameChanged(ev);
                _gameChangedSignal.Set();
            });
            hub.On<GameEndedEvent>("GameEnded", ev => {
                GameEnded(ev);
                GameEndedSignal.Set();
            });
            hub.On<GameStartedEvent>("GameStarted", ev => {
                GameStarted(ev);
                GameStartedSignal.Set();
            });
            hub.On<PlayerJoinedEvent>("PlayerJoined", ev => {
                PlayerJoined(ev);
                PlayerJoinedSignal.Set();
            });
            hub.On<PlayerLeftEvent>("PlayerLeft", ev => {
                PlayerLeft(ev);
                PlayerLeftSignal.Set();
            });
        }

        public Task CreateGame(CreateGameCmd cmd) {
            GameCreatedSignal.Reset();
            return InvokeMeAsync(cmd, false);
        }

        public Task JoinGame(GenericGameCmd cmd) {
            PlayerJoinedSignal.Reset();
            return InvokeMeAsync(cmd, false);
        }

        public Task LeaveGame(GenericGameCmd cmd) {
            PlayerLeftSignal.Reset();
            return InvokeMeAsync(cmd, false);
        }

        public Task StartGame(GenericGameCmd cmd) {
            GameStartedSignal.Reset();
            return InvokeMeAsync(cmd);
        }

        public Task SelectRole(GameCommand<SelectRole> cmd) {
            return InvokeMeAsync(cmd);
        }

        public Task BonusProduction(GameCommand<BonusProduction> cmd) {
            return InvokeMeAsync(cmd);
        }

        public Task Build(GameCommand<Build> cmd) {
            return InvokeMeAsync(cmd);
        }

        public Task DeliverGoods(GameCommand<DeliverGoods> cmd) {
            return InvokeMeAsync(cmd);
        }

        public Task EndPhase(GameCommand<EndPhase> cmd) {
            return InvokeMeAsync(cmd);
        }

        public Task EndRole(GameCommand<EndRole> cmd) {
            return InvokeMeAsync(cmd);
        }

        public Task MoveColonist(GameCommand<MoveColonist> cmd) {
            return InvokeMeAsync(cmd);
        }

        public Task PlaceColonist(GameCommand<PlaceColonist> cmd) {
            return InvokeMeAsync(cmd);
        }

        public Task SellGood(GameCommand<SellGood> cmd) {
            return InvokeMeAsync(cmd);
        }

        public Task StoreGoods(GameCommand<StoreGoods> cmd) {
            return InvokeMeAsync(cmd);
        }

        public Task TakePlantation(GameCommand<TakePlantation> cmd) {
            return InvokeMeAsync(cmd);
        }

        public Task TakeQuarry(GameCommand<TakeQuarry> cmd) {
            return InvokeMeAsync(cmd);
        }

        public Task TakeRandomPlantation(GameCommand<TakeRandomPlantation> cmd) {
            return InvokeMeAsync(cmd);
        }

        public Task UseWharf(GameCommand<UseWharf> cmd) {
            return InvokeMeAsync(cmd);
        }

        private async Task InvokeMeAsync(object arg1, bool waitForGameChange = true,
            [CallerMemberName] string caller = null) {
            SetFakeUser();
            if (waitForGameChange) {
                _gameChangedSignal.Reset();
                var t = _hub.SendAsync(caller, arg1);
                _gameChangedSignal.WaitOne(5000);
                await t;
            }
            else {
                await _hub.InvokeAsync(caller, arg1);
            }

            await Task.Delay(300);
        }

        private void SetFakeUser() {
            _factory.FakeUserService.UserId = UserId;
        }
    }
}