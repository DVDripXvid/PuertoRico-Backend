using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuertoRico.Engine.Domain;
using PuertoRico.Engine.Domain.Player;
using PuertoRico.Engine.Exceptions;
using PuertoRico.Engine.Extensions;

namespace PuertoRico.Engine.Stores.InMemory
{
    public class InMemoryGameStore : IGameStore
    {
        private readonly Dictionary<string, Game> _games = new Dictionary<string, Game>();

        public Game FindById(string gameId) {
            if (_games.ContainsKey(gameId)) {
                return _games[gameId];
            }

            throw new GameException("Game not found");
        }

        public IEnumerable<Game> FindNotStarted() {
            return _games.Values.Where(g => !g.IsStarted);
        }

        public Task Add(Game game) {
            if (_games.ContainsKey(game.Id)) {
                throw new InvalidOperationException("Game already added with id = " + game.Id);
            }

            _games[game.Id] = game;
            return Task.CompletedTask;
        }

        public Task<Game> Remove(string gameId) {
            if (_games.TryGetValue(gameId, out var game)) {
                _games.Remove(gameId);
                return Task.FromResult(game);
            }

            throw new InvalidOperationException($"Game with id = {gameId} does not exist");
        }

        public IEnumerable<Game> FindByUserId(string userId) {
            return _games.Values.Where(g => g.Players.Any(p => p.UserId == userId));
        }

        public Task JoinGame(string gameId, IPlayer player) {
            var game = FindById(gameId);
            game.Join(player);
            return Task.CompletedTask;
        }

        public Task<IPlayer> LeaveGame(string gameId, string userId) {
            var game = FindById(gameId);
            var player = game.Players.WithUserId(userId);
            game.Leave(player);
            return Task.FromResult(player);
        }
    }
}