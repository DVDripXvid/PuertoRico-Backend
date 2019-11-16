using System;
using System.Collections.Generic;
using System.Linq;
using PuertoRico.Engine.Domain;
using PuertoRico.Engine.Exceptions;

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

        public IEnumerable<Game> FindNotFull() {
            return _games.Values.Where(g => !g.IsFull);
        }

        public void Add(Game game) {
            if (_games.ContainsKey(game.Id)) {
                throw new InvalidOperationException("Game already added with id = " + game.Id);
            }

            _games[game.Id] = game;
        }

        public void Remove(string gameId) {
            _games.Remove(gameId);
        }

        public IEnumerable<Game> FindByUserId(string userId) {
            return _games.Values.Where(g => g.Players.Any(p => p.UserId == userId));
        }
    }
}