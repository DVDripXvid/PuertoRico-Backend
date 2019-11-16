using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain;
using PuertoRico.Engine.Domain.Player;
using PuertoRico.Engine.DTOs;
using PuertoRico.Engine.Events;
using PuertoRico.Engine.Extensions;
using PuertoRico.Engine.Gameplay;
using PuertoRico.Engine.Stores;

namespace PuertoRico.Engine.SignalR
{
    public class GameHub : Hub
    {
        private readonly IGameService _gameService;
        private readonly IGameStore _gameStore;
        private const string LobbyGroup = "Lobby"; 

        public GameHub(IGameService gameService, IGameStore gameStore) {
            _gameService = gameService;
            _gameStore = gameStore;
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, LobbyGroup);
            var games = _gameStore.FindByUserId(GetUserId()).ToList();
            foreach (var game in games)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, game.Id);
                if (game.IsStarted)
                {
                    var gameChangedEvent = new GameChangedEvent
                    {
                        Game = new GameDto(game)
                    };
                    await Clients.Caller.SendAsync("gameChanged", gameChangedEvent);
                }
            }
        }

        public async Task CreateGame(string name) {
            var player = CreatePlayerForCurrentUser();
            var game = new Game(name);
            game.Join(player);
            await Groups.AddToGroupAsync(Context.ConnectionId, game.Id);
            var gameCreatedEvent = new GameCreatedEvent
            {
                GameId = game.Id,
                GameName = game.Name,
                CreatedBy = new PlayerDto(GetUserName(), player)
            };
            await Clients.Group(LobbyGroup).SendAsync("gameCreated", gameCreatedEvent);
        }

        public async Task JoinGame(string gameId) {
            var player = CreatePlayerForCurrentUser();
            var game = _gameStore.FindById(gameId);
            game.Join(player);
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            var joinedEvent = new PlayerJoinedEvent
            {
                Player = new PlayerDto(GetUserName(), player),
                GameId = gameId
            };
            await Clients.Group(gameId).SendAsync("playerJoined", joinedEvent);
        }

        public async Task LeaveGame(string gameId) {
            var game = _gameStore.FindById(gameId);
            var player = game.Players.WithUserId(GetUserId());
            game.Leave(player);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
            var leftEvent = new PlayerLeftEvent()
            {
                Player = new PlayerDto(GetUserName(), player),
                GameId = gameId
            };
            await Clients.Group(gameId).SendAsync("playerLeft", leftEvent);
        }

        public async Task StartGame(string gameId) {
            var game = _gameStore.FindById(gameId);
            game.Start();
            var startedEvent = new GameStartedEvent
            {
                GameId = gameId
            };
            await Clients.Group(LobbyGroup).SendAsync("gameStarted", startedEvent);
            var changedEvent = new GameChangedEvent
            {
                Game = new GameDto(game)
            };
            await Clients.Group(gameId).SendAsync("gameChanged", game);
        }

        public async Task Build(string gameId, Build build) {
            await ExecuteRoleAction(gameId, build);
            //TODO: raise built event
        }

        private async Task ExecuteRoleAction(string gameId, IAction build) {
            var game = _gameStore.FindById(gameId);
            await _gameService.ExecuteRoleAction(game, GetUserId(), build);
            if (game.IsEnded) {
                //TODO: raise game ended event
            }
        }

        private IPlayer CreatePlayerForCurrentUser() {
            return new Player(GetUserId());
        }

        private string GetUserId() {
            return Context.UserIdentifier;
        }

        private string GetUserName()
        {
            return Context.User.FindFirstValue(ClaimTypes.Name) ?? Context.UserIdentifier;
        }
    }
}