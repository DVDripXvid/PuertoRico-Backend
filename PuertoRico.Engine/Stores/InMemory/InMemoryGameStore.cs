using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.DAL;
using PuertoRico.Engine.Domain;
using PuertoRico.Engine.Domain.Player;
using PuertoRico.Engine.Domain.Tiles.Plantations;
using PuertoRico.Engine.Exceptions;
using PuertoRico.Engine.Extensions;
using PuertoRico.Engine.Services;

namespace PuertoRico.Engine.Stores.InMemory
{
    public class InMemoryGameStore : IGameStore
    {
        private readonly Dictionary<string, Game> _games = new Dictionary<string, Game>();
        private readonly IGameRepository _repository;
        private readonly IGameService _gameService;
        
        public InMemoryGameStore(IGameRepository repository, IGameService gameService) {
            _repository = repository;
            _gameService = gameService;
            InitializeGamesFromDb().Wait();
        }

        public Game FindById(string gameId) {
            if (_games.ContainsKey(gameId)) {
                return _games[gameId];
            }

            throw new GameException("Game not found");
        }

        public IEnumerable<Game> FindNotStarted() {
            return _games.Values.Where(g => !g.IsStarted);
        }

        public async Task Add(Game game) {
            if (_games.ContainsKey(game.Id)) {
                throw new InvalidOperationException("Game already added with id = " + game.Id);
            }

            await _repository.CreateGame(GameEntity.Create(game));
            _games[game.Id] = game;
        }

        public async Task<Game> Remove(string gameId) {
            if (_games.TryGetValue(gameId, out var game)) {
                _games.Remove(gameId);
                await _repository.DeleteGame(gameId, game.RandomSeed);
                return game;
            }

            throw new InvalidOperationException($"Game with id = {gameId} does not exist");
        }

        public IEnumerable<Game> FindByUserId(string userId) {
            return _games.Values.Where(g => g.Players.Any(p => p.UserId == userId));
        }

        public async Task JoinGame(string gameId, IPlayer player) {
            var game = FindById(gameId);
            game.Join(player);

            await _repository.ReplaceGame(GameEntity.Create(game));
        }

        public async Task<IPlayer> LeaveGame(string gameId, string userId) {
            var game = FindById(gameId);
            var player = game.Players.WithUserId(userId);
            game.Leave(player);
            
            await _repository.ReplaceGame(GameEntity.Create(game));
            return player;
        }

        private async Task InitializeGamesFromDb() {
            var lobbyGames = await _repository.GetLobbyGames();
            foreach (var lobbyGame in lobbyGames) {
                var game = new Game(lobbyGame.Id, lobbyGame.Name, lobbyGame.RandomSeed);
                lobbyGame.Players.ToList().ForEach(p => game.Join(new Player(p.UserId, p.Username, p.PictureUrl)));
                _games.Add(game.Id, game);
            }

            var games = await _repository.GetStartedGames();
            foreach (var gameEntity in games) {
                var game = new Game(gameEntity.Id, gameEntity.Name, gameEntity.RandomSeed);
                gameEntity.Players.ToList().ForEach(p => game.Join(new Player(p.UserId, p.Username, p.PictureUrl)));
                game.Start();
                var actions = await _repository.GetActionsByGame(game.Id);
                actions.ToList().ForEach(a => {
                    if (a.Action is SelectRole selectRole) {
                        _gameService.ExecuteSelectRole(game, a.UserId, selectRole);
                    }
                    else {
                        _gameService.ExecuteRoleAction(game, a.UserId, a.Action);
                    }
                });
                _games.Add(game.Id, game);
            }
        }
    }
}