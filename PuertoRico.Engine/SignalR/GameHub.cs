using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain;
using PuertoRico.Engine.Domain.Player;
using PuertoRico.Engine.Extensions;
using PuertoRico.Engine.Gameplay;
using PuertoRico.Engine.Stores;

namespace PuertoRico.Engine.SignalR
{
    public class GameHub : Hub
    {
        private readonly IGameService _gameService;
        private readonly IGameStore _gameStore;

        public GameHub(IGameService gameService, IGameStore gameStore) {
            _gameService = gameService;
            _gameStore = gameStore;
        }

        public void CreateGame(string name) {
            var player = CreatePlayerForCurrentUser();
            var game = new Game(name);
            game.Join(player);
            //TODO: raise created event
        }

        public void JoinGame(string gameId) {
            var player = CreatePlayerForCurrentUser();
            var game = _gameStore.FindById(gameId);
            game.Join(player);
            //TODO: raise joined event
        }

        public void LeaveGame(string gameId) {
            var game = _gameStore.FindById(gameId);
            var player = game.Players.WithUserId(GetUserId());
            game.Leave(player);
            //TODO: raise left event
        }

        public void StartGame(string gameId) {
            var game = _gameStore.FindById(gameId);
            game.Start();
            // TODO: raise started event
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
    }
}