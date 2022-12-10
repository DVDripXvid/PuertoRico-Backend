using Microsoft.Extensions.Configuration;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PuertoRico.Engine.DAL.InMemory
{
    public class InMemoryGameRepository : IGameRepository
    {
        private readonly string _gameEndpoint;

        private static readonly Dictionary<string, GameEntity> Games = new();

        public InMemoryGameRepository(IConfiguration configuration)
        {
            _gameEndpoint = configuration["GameEndpoint"];
        }

        public Task AddAction(string gameId, string playerId, IAction action)
        {
            // event sourcing not applicable for in-memory repositoriy
            return Task.CompletedTask;
        }

        public Task CreateGame(GameEntity gameEntity)
        {
            Games.Add(gameEntity.Id, gameEntity);
            return Task.CompletedTask;
        }

        public Task DeleteGame(string gameId)
        {
            Games.Remove(gameId);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<ActionEntity>> GetActionsByGame(string gameId)
        {
            // event sourcing not applicable for in-memory repositoriy
            return Task.FromResult(Enumerable.Empty<ActionEntity>());
        }

        public Task<GameEntity> GetGame(string gameId)
        {
            return Task.FromResult(Games[gameId]);
        }

        public Task<IEnumerable<GameEntity>> GetInProgressGamesForCurrentApplication()
        {
            return Task.FromResult(
                Games.Values.Where(g => g.Status == GameStatus.RUNNING && g.Endpoint == _gameEndpoint));
        }

        public Task<IEnumerable<GameEntity>> GetLobbyGames()
        {
            return Task.FromResult(
                Games.Values.Where(g => g.Status == GameStatus.INITIAL));
        }

        public Task<IEnumerable<GameEntity>> GetStartedGamesByPlayer(string userId)
        {
            return Task.FromResult(
                Games.Values.Where(g => g.Status != GameStatus.INITIAL && g.Players.Any(p => p.UserId == userId)));
        }

        public Task ReplaceGame(GameEntity gameEntity)
        {
            Games[gameEntity.Id] = gameEntity;
            return Task.CompletedTask;
        }
    }
}
