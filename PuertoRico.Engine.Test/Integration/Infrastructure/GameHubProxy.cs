using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.DTOs;
using PuertoRico.Engine.Events;
using PuertoRico.Engine.SignalR;

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

        public Task CreateGame(string name) {
            GameCreatedSignal.Reset();
            return InvokeMeAsync(name,false);
        }

        public Task JoinGame(string gameId) {
            PlayerJoinedSignal.Reset();
            return InvokeMeAsync(gameId, false);
        }

        public Task LeaveGame(string gameId) {
            PlayerLeftSignal.Reset();
            return InvokeMeAsync(gameId, false);
        }

        public Task StartGame(string gameId) {
            GameStartedSignal.Reset();
            return InvokeMeAsync(gameId);
        }

        public Task SelectRole(string gameId, SelectRole selectRole) {
            return InvokeMeAsync(gameId, selectRole);
        }

        public Task BonusProduction(string gameId, BonusProduction action) {
            return InvokeMeAsync(gameId, action);
        }

        public Task Build(string gameId, Build action) {
            return InvokeMeAsync(gameId, action);
        }

        public Task DeliverGoods(string gameId, DeliverGoods action) {
            return InvokeMeAsync(gameId, action);
        }

        public Task EndPhase(string gameId, EndPhase action) {
            return InvokeMeAsync(gameId, action);
        }

        public Task EndRole(string gameId, EndRole action) {
            return InvokeMeAsync(gameId, action);
        }

        public Task MoveColonist(string gameId, MoveColonist action) {
            return InvokeMeAsync(gameId, action);
        }

        public Task SellGood(string gameId, SellGood action) {
            return InvokeMeAsync(gameId, action);
        }

        public Task StoreGoods(string gameId, StoreGoods action) {
            return InvokeMeAsync(gameId, action);
        }

        public Task TakePlantation(string gameId, TakePlantation action) {
            return InvokeMeAsync(gameId, action);
        }

        public Task TakeQuarry(string gameId, TakeQuarry action) {
            return InvokeMeAsync(gameId, action);
        }

        public Task TakeRandomPlantation(string gameId, TakeRandomPlantation action) {
            return InvokeMeAsync(gameId, action);
        }

        public Task UseWharf(string gameId, UseWharf action) {
            return InvokeMeAsync(gameId, action);
        }

        private async Task InvokeMeAsync(object arg1, bool waitForGameChange = true, [CallerMemberName] string caller = null) {
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

        private async Task InvokeMeAsync(object arg1, object arg2,  bool waitForGameChange = true, [CallerMemberName] string caller = null) {
            SetFakeUser();
            if (waitForGameChange) {
                _gameChangedSignal.Reset();
                var t = _hub.SendAsync(caller, arg1, arg2);
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