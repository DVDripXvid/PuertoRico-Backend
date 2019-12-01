using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain;
using PuertoRico.Engine.Domain.Player;
using PuertoRico.Engine.DTOs;
using PuertoRico.Engine.Events;
using PuertoRico.Engine.Extensions;
using PuertoRico.Engine.Services;
using PuertoRico.Engine.Stores;

namespace PuertoRico.Engine.SignalR
{
    public class GameHub : Hub<IGameClient>, IGameHub
    {
        private readonly IGameService _gameService;
        private readonly IGameStore _gameStore;
        private readonly IUserService _userService;
        private const string LobbyGroup = "Lobby";

        public GameHub(IGameService gameService, IGameStore gameStore, IUserService userService) {
            _gameService = gameService;
            _gameStore = gameStore;
            _userService = userService;
        }

        public override async Task OnConnectedAsync() {
            await Groups.AddToGroupAsync(Context.ConnectionId, LobbyGroup);
            var games = _gameStore.FindByUserId(GetUserId()).ToList();
            foreach (var game in games) {
                await Groups.AddToGroupAsync(Context.ConnectionId, game.Id);
                if (game.IsStarted) {
                    var gameChangedEvent = new GameChangedEvent {
                        Game = GameDto.Create(game)
                    };
                    await Clients.Caller.GameChanged(gameChangedEvent);
                }
            }
        }

        public async Task CreateGame(string name) {
            var player = CreatePlayerForCurrentUser();
            var game = new Game(name);
            _gameStore.Add(game);
            game.Join(player);
            var gameCreatedEvent = new GameCreatedEvent {
                GameId = game.Id,
                GameName = game.Name,
                CreatedBy = PlayerDto.Create(player)
            };
            await Clients.Group(LobbyGroup).GameCreated(gameCreatedEvent);
            await Groups.AddToGroupAsync(Context.ConnectionId, game.Id);
        }

        public async Task JoinGame(string gameId) {
            var player = CreatePlayerForCurrentUser();
            var game = _gameStore.FindById(gameId);
            game.Join(player);
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            var joinedEvent = new PlayerJoinedEvent {
                Player = PlayerDto.Create(player),
                GameId = gameId
            };
            await Clients.Group(gameId).PlayerJoined(joinedEvent);
        }

        public async Task LeaveGame(string gameId) {
            var game = _gameStore.FindById(gameId);
            var player = game.Players.WithUserId(GetUserId());
            game.Leave(player);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
            var leftEvent = new PlayerLeftEvent() {
                Player = PlayerDto.Create(player),
                GameId = gameId
            };
            await Clients.Group(gameId).PlayerLeft(leftEvent);
        }

        public async Task StartGame(string gameId) {
            var game = _gameStore.FindById(gameId);
            game.Start();
            var startedEvent = new GameStartedEvent {
                GameId = gameId
            };
            await Clients.Group(LobbyGroup).GameStarted(startedEvent);
            var changedEvent = new GameChangedEvent {
                Game = GameDto.Create(game)
            };
            await Clients.Group(gameId).GameChanged(changedEvent);
        }

        public async Task SelectRole(string gameId, SelectRole selectRole) {
            var game = _gameStore.FindById(gameId);
            await _gameService.ExecuteSelectRole(game, GetUserId(), selectRole);
            //TODO: raise role selected event
            var changedEvent = new GameChangedEvent {
                Game = GameDto.Create(game)
            };
            await Clients.Group(gameId).GameChanged(changedEvent);
        }

        public async Task BonusProduction(string gameId, BonusProduction bonusProduction) {
            await ExecuteRoleAction(gameId, bonusProduction);
        }

        public async Task Build(string gameId, Build build) {
            await ExecuteRoleAction(gameId, build);
        }

        public async Task DeliverGoods(string gameId, DeliverGoods deliverGoods) {
            await ExecuteRoleAction(gameId, deliverGoods);
        }

        public async Task EndPhase(string gameId, EndPhase endPhase) {
            await ExecuteRoleAction(gameId, endPhase);
        }

        public async Task EndRole(string gameId, EndRole endRole) {
            await ExecuteRoleAction(gameId, endRole);
        }

        public async Task MoveColonist(string gameId, MoveColonist moveColonist) {
            await ExecuteRoleAction(gameId, moveColonist);
        }

        public async Task SellGood(string gameId, SellGood sellGood) {
            await ExecuteRoleAction(gameId, sellGood);
        }

        public async Task StoreGoods(string gameId, StoreGoods storeGoods) {
            await ExecuteRoleAction(gameId, storeGoods);
        }

        public async Task TakePlantation(string gameId, TakePlantation takePlantation) {
            await ExecuteRoleAction(gameId, takePlantation);
        }

        public async Task TakeQuarry(string gameId, TakeQuarry takeQuarry) {
            await ExecuteRoleAction(gameId, takeQuarry);
        }

        public async Task TakeRandomPlantation(string gameId, TakeRandomPlantation takeRandomPlantation) {
            await ExecuteRoleAction(gameId, takeRandomPlantation);
        }

        public async Task UseWharf(string gameId, UseWharf useWharf) {
            await ExecuteRoleAction(gameId, useWharf);
        }

        private async Task ExecuteRoleAction(string gameId, IAction build) {
            var game = _gameStore.FindById(gameId);
            await _gameService.ExecuteRoleAction(game, GetUserId(), build);
            //TODO: may be a good idea to specify events related to executed action
            await Clients.Groups(gameId).GameChanged(new GameChangedEvent {Game = GameDto.Create(game)});
            if (game.IsEnded) {
                await AfterGameEnded(game);
            }
        }

        private async Task AfterGameEnded(Game game) {
            var endedEvent = new GameEndedEvent {
                GameId = game.Id
            };
            await Clients.Groups(game.Id).GameEnded(endedEvent);
            _gameStore.Remove(game.Id);
        }

        private IPlayer CreatePlayerForCurrentUser() {
            return new Player(GetUserId(), GetUserName());
        }

        private string GetUserId() {
            return _userService.GetUserId(Context);
        }

        private string GetUserName() {
            return _userService.GetUsername(Context);
        }
    }
}