using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain;
using PuertoRico.Engine.Domain.Player;
using PuertoRico.Engine.DTOs;
using PuertoRico.Engine.Exceptions;
using PuertoRico.Engine.Extensions;
using PuertoRico.Engine.Services;
using PuertoRico.Engine.SignalR.Commands;
using PuertoRico.Engine.SignalR.Events;
using PuertoRico.Engine.Stores;

namespace PuertoRico.Engine.SignalR
{
    [Authorize]
    public class GameHub : Hub<IGameClient>, IGameHub
    {
        private readonly IReplayableGameService _gameService;
        private readonly IGameStore _gameStore;
        private readonly IUserService _userService;
        private const string LobbyGroup = "Lobby";

        public GameHub(IReplayableGameService gameService, IGameStore gameStore, IUserService userService) {
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
                    await Clients.Caller.GameStarted(new GameStartedEvent {GameId = game.Id});
                    var gameChangedEvent = new GameChangedEvent {
                        Game = GameDto.Create(game)
                    };
                    await Clients.Caller.GameChanged(gameChangedEvent);
                    await SendAvailableActionTypes(game, GetUserId());
                }
            }

            var lobbyGames = _gameStore.FindNotStarted();
            foreach (var lobbyGame in lobbyGames) {
                var gameCreatedEvent = new GameCreatedEvent {
                    GameId = lobbyGame.Id,
                    GameName = lobbyGame.Name,
                    Players = lobbyGame.Players.Select(PlayerDto.Create),
                };
                await Clients.Caller.GameCreated(gameCreatedEvent);
            }
        }

        public async Task CreateGame(CreateGameCmd cmd) {
            var player = CreatePlayerForCurrentUser();
            var game = new Game(cmd.Name);
            await _gameStore.Add(game);
            await _gameStore.JoinGame(game.Id, player);
            var gameCreatedEvent = new GameCreatedEvent {
                GameId = game.Id,
                GameName = game.Name,
                Players = game.Players.Select(PlayerDto.Create)
            };
            await Clients.Group(LobbyGroup).GameCreated(gameCreatedEvent);
            await Groups.AddToGroupAsync(Context.ConnectionId, game.Id);
        }

        public async Task JoinGame(GenericGameCmd cmd) {
            var gameId = cmd.GameId;
            var player = CreatePlayerForCurrentUser();
            await _gameStore.JoinGame(gameId, player);
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            var joinedEvent = new PlayerJoinedEvent {
                Player = PlayerDto.Create(player),
                GameId = gameId
            };
            await Clients.Group(LobbyGroup).PlayerJoined(joinedEvent);
        }

        public async Task LeaveGame(GenericGameCmd cmd) {
            var gameId = cmd.GameId;
            var game = _gameStore.FindById(gameId);
            var player = await _gameStore.LeaveGame(gameId, GetUserId());
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
            if (game.Players.Any()) {
                var leftEvent = new PlayerLeftEvent() {
                    Player = PlayerDto.Create(player),
                    GameId = gameId
                };
                await Clients.Group(LobbyGroup).PlayerLeft(leftEvent);
            }
            else {
                await _gameStore.Remove(gameId);
                var removedEvent = new GameDestroyedEvent() {
                    GameId = gameId
                };
                await Clients.Group(LobbyGroup).GameDestroyed(removedEvent);
            }
        }

        public async Task StartGame(GenericGameCmd cmd) {
            var gameId = cmd.GameId;
            var game = _gameStore.FindById(gameId);
            await _gameService.StartGame(game);
            var startedEvent = new GameStartedEvent {
                GameId = gameId
            };
            await Clients.Group(LobbyGroup).GameStarted(startedEvent);
            var changedEvent = new GameChangedEvent {
                Game = GameDto.Create(game)
            };
            await Clients.Group(gameId).GameChanged(changedEvent);

            var tasks = game.Players.Select(p => SendAvailableActionTypes(game, p.UserId));
            await Task.WhenAll(tasks);
        }

        public async Task SelectRole(GameCommand<SelectRole> cmd) {
            var game = _gameStore.FindById(cmd.GameId);
            try {
                await _gameService.ExecuteSelectRole(game, GetUserId(), cmd.Action);
            }
            catch (Exception e) {
                await Clients.Caller.Error(new GameErrorEvent {ErrorMessage = e.Message});
            }

            //TODO: raise role selected event
            var changedEvent = new GameChangedEvent {
                Game = GameDto.Create(game)
            };
            await Clients.Group(cmd.GameId).GameChanged(changedEvent);

            await SendAvailableActionTypes(game, GetUserId());
        }

        public Task BonusProduction(GameCommand<BonusProduction> cmd) {
            return ExecuteRoleAction(cmd.GameId, cmd.Action);
        }

        public Task Build(GameCommand<Build> cmd) {
            return ExecuteRoleAction(cmd.GameId, cmd.Action);
        }

        public Task DeliverGoods(GameCommand<DeliverGoods> cmd) {
            return ExecuteRoleAction(cmd.GameId, cmd.Action);
        }

        public Task EndPhase(GameCommand<EndPhase> cmd) {
            return ExecuteRoleAction(cmd.GameId, cmd.Action);
        }

        public Task EndRole(GameCommand<EndRole> cmd) {
            return ExecuteRoleAction(cmd.GameId, cmd.Action);
        }

        public Task MoveColonist(GameCommand<MoveColonist> cmd) {
            return ExecuteRoleAction(cmd.GameId, cmd.Action);
        }

        public Task PlaceColonist(GameCommand<PlaceColonist> cmd) {
            return ExecuteRoleAction(cmd.GameId, cmd.Action);
        }

        public Task SellGood(GameCommand<SellGood> cmd) {
            return ExecuteRoleAction(cmd.GameId, cmd.Action);
        }

        public Task StoreGoods(GameCommand<StoreGoods> cmd) {
            return ExecuteRoleAction(cmd.GameId, cmd.Action);
        }

        public Task TakePlantation(GameCommand<TakePlantation> cmd) {
            return ExecuteRoleAction(cmd.GameId, cmd.Action);
        }

        public Task TakeQuarry(GameCommand<TakeQuarry> cmd) {
            return ExecuteRoleAction(cmd.GameId, cmd.Action);
        }

        public Task TakeRandomPlantation(GameCommand<TakeRandomPlantation> cmd) {
            return ExecuteRoleAction(cmd.GameId, cmd.Action);
        }

        public Task UseWharf(GameCommand<UseWharf> cmd) {
            return ExecuteRoleAction(cmd.GameId, cmd.Action);
        }

        private async Task ExecuteRoleAction(string gameId, IAction build) {
            var game = _gameStore.FindById(gameId);
            try {
                await _gameService.ExecuteRoleAction(game, GetUserId(), build);
            }
            catch (Exception e) {
                await Clients.Caller.Error(new GameErrorEvent {ErrorMessage = e.Message});
            }

            //TODO: may be a good idea to specify events related to executed action
            await Clients.Groups(gameId).GameChanged(new GameChangedEvent {Game = GameDto.Create(game)});

            var tasks = game.Players.Select(p => SendAvailableActionTypes(game, p.UserId));
            await Task.WhenAll(tasks);

            if (game.IsEnded) {
                await AfterGameEnded(game);
            }
        }

        private async Task SendAvailableActionTypes(Game game, string userId) {
            var availableActions = await _gameService.GetAvailableActionTypeForUser(game, userId);

            await Clients.Users(userId).AvailableActionTypesChanged(new AvailableActionTypesChangedEvent {
                GameId = game.Id,
                ActionTypes = availableActions,
            });
        }

        private async Task AfterGameEnded(Game game) {
            var endedEvent = new GameEndedEvent {
                GameId = game.Id
            };
            await Clients.Groups(game.Id).GameEnded(endedEvent);
            await _gameStore.Remove(game.Id);
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