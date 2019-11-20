using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using PuertoRico.Engine.Events;
using PuertoRico.Engine.SignalR;

namespace PuertoRico.Engine.Test.Integration.Infrastructure
{
    public class GameHubProxy : IGameHub
    {
        public event Action<GameCreatedEvent> GameCreated;

        public event Action<GameChangedEvent> GameChanged;

        public event Action<GameEndedEvent> GameEnded;

        public event Action<GameStartedEvent> GameStarted;

        public event Action<PlayerJoinedEvent> PlayerJoined;

        public event Action<PlayerLeftEvent> PlayerLeft;

        public readonly Stack<IGameEvent> EventStack = new Stack<IGameEvent>();

        private readonly HubConnection _hub;
        private readonly string _userId;
        private readonly TestWebApplicationFactory _factory;

        public GameHubProxy(HubConnection hub, TestWebApplicationFactory factory, string userId)
        {
            _hub = hub;
            _factory = factory;
            _userId = userId;
            hub.On<GameCreatedEvent>("GameCreated", ev => GameCreated(ev));
            hub.On<GameChangedEvent>("GameChanged", ev => GameChanged(ev));
            hub.On<GameEndedEvent>("GameEnded", ev => GameEnded(ev));
            hub.On<GameStartedEvent>("GameStarted", ev => GameStarted(ev));
            hub.On<PlayerJoinedEvent>("PlayerJoined", ev => PlayerJoined(ev));
            hub.On<PlayerLeftEvent>("PlayerLeft", ev => PlayerLeft(ev));

            GameCreated += EventStack.Push;
            GameChanged += EventStack.Push;
            GameEnded += EventStack.Push;
            GameStarted += EventStack.Push;
            PlayerJoined += EventStack.Push;
            PlayerLeft += EventStack.Push;
        }
        
        public async Task CreateGame(string name)
        {
            SetFakeUser();
            await _hub.InvokeAsync("CreateGame", name);
            Console.WriteLine("success");
        }

        public Task JoinGame(string gameId)
        {
            return InvokeMeAsync(gameId);
        }

        public Task LeaveGame(string gameId)
        {
            return InvokeMeAsync(gameId);
        }

        public Task StartGame(string gameId)
        {
            return InvokeMeAsync(gameId);
        }

        private async Task InvokeMeAsync(object arg1, [CallerMemberName] string caller = null)
        {
            SetFakeUser();
            await _hub.InvokeAsync(caller, arg1);
            Console.WriteLine("success");
        }

        private Task InvokeMeAsync(object arg1, object arg2, [CallerMemberName] string caller = null)
        {
            SetFakeUser();
            return _hub.InvokeAsync(caller, arg1);
        }

        private void SetFakeUser()
        {
            A.CallTo(() => _factory.FakeUserService.GetUserId(A<HubCallerContext>.Ignored))
                .Returns(_userId);
            A.CallTo(() => _factory.FakeUserService.GetUsername(A<HubCallerContext>.Ignored))
                .Returns(_userId);
        }
    }
}