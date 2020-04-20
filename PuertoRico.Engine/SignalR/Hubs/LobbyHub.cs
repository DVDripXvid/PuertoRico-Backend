using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using PuertoRico.Engine.DAL;
using PuertoRico.Engine.Domain;
using PuertoRico.Engine.Domain.Player;
using PuertoRico.Engine.DTOs;
using PuertoRico.Engine.Extensions;
using PuertoRico.Engine.Services;
using PuertoRico.Engine.SignalR.Commands;
using PuertoRico.Engine.SignalR.Events;
using PuertoRico.Engine.Stores;

namespace PuertoRico.Engine.SignalR.Hubs
{
    [Authorize]
    public class LobbyHub : Hub<ILobbyClient>, ILobbyHub
    {
        private readonly IGameRepository _repository;
        private readonly IInProgressGameStore _inprogressGameStore;
        private readonly IUserService _userService;
        private const string LobbyGroup = "Lobby";
        private readonly string _endpointUrl;

        public LobbyHub(IGameRepository repository, IUserService userService, IInProgressGameStore inprogressGameStore,
            IConfiguration configuration) {
            _repository = repository;
            _userService = userService;
            _inprogressGameStore = inprogressGameStore;
            _endpointUrl = configuration["PublicUrl"] + "/game";
        }

        public override async Task OnConnectedAsync() {
            await Groups.AddToGroupAsync(Context.ConnectionId, LobbyGroup);
            var games = await _repository.GetStartedGamesByPlayer(GetUserId());
            foreach (var game in games.Select(g => g.ToModel())) {
                await Clients.Caller.GameStarted(new GameStartedEvent {Game = GameSummaryDto.Create(game, _endpointUrl)});
            }

            var lobbyGames = await _repository.GetLobbyGames();
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
            var game = new Game(cmd.Name);
            var player = CreatePlayerForCurrentUser();
            game.Join(player);
            await _repository.CreateGame(GameEntity.Create(game));
            var gameCreatedEvent = new GameCreatedEvent {
                GameId = game.Id,
                GameName = game.Name,
                Players = game.Players.Select(PlayerDto.Create)
            };
            await Clients.Group(LobbyGroup).GameCreated(gameCreatedEvent);
        }

        public async Task JoinGame(GenericGameCmd cmd) {
            var gameEntity = await _repository.GetGame(cmd.GameId);
            var game = gameEntity.ToModel();
            var player = CreatePlayerForCurrentUser();
            game.Join(player);
            await _repository.ReplaceGame(GameEntity.Create(game));

            var joinedEvent = new PlayerJoinedEvent {
                Player = PlayerDto.Create(player),
                GameId = game.Id
            };
            await Clients.Group(LobbyGroup).PlayerJoined(joinedEvent);
        }

        public async Task LeaveGame(GenericGameCmd cmd) {
            var gameEntity = await _repository.GetGame(cmd.GameId);
            var game = gameEntity.ToModel();
            var player = game.Players.WithUserId(GetUserId());
            game.Leave(player);

            if (game.Players.Any()) {
                await _repository.ReplaceGame(GameEntity.Create(game));
                var leftEvent = new PlayerLeftEvent() {
                    Player = PlayerDto.Create(player),
                    GameId = game.Id
                };
                await Clients.Group(LobbyGroup).PlayerLeft(leftEvent);
            }
            else {
                await _repository.DeleteGame(game.Id);
                var removedEvent = new GameDestroyedEvent() {
                    GameId = game.Id
                };
                await Clients.Group(LobbyGroup).GameDestroyed(removedEvent);
            }
        }

        public async Task StartGame(GenericGameCmd cmd) {
            var gameEntity = await _repository.GetGame(cmd.GameId);
            var game = gameEntity.ToModel();
            game.Start();

            await _repository.ReplaceGame(GameEntity.Create(game));
            _inprogressGameStore.Add(game);

            var startedEvent = new GameStartedEvent {
                Game = GameSummaryDto.Create(game, _endpointUrl)
            };
            await Clients.Group(LobbyGroup).GameStarted(startedEvent);
        }

        private IPlayer CreatePlayerForCurrentUser() {
            return new Player(GetUserId(), GetUsername(), GetPictureUrl());
        }

        private string GetUserId() {
            return _userService.GetUserId(Context);
        }

        private string GetUsername() {
            return _userService.GetUsername(Context);
        }

        private string GetPictureUrl() {
            return _userService.GetPictureUrl(Context);
        }
    }
}