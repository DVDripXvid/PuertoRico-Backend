using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain;

namespace PuertoRico.Engine.DAL
{
    public class CosmosGameRepository : IGameRepository
    {
        private readonly Container _actionsContainer;
        private readonly Container _gamesContainer;
        private readonly ILogger<CosmosGameRepository> _logger;
        private readonly string _gameEndpoint;

        public CosmosGameRepository(CosmosClient dbClient, ILogger<CosmosGameRepository> logger, IConfiguration configuration) {
            _logger = logger;
            _gameEndpoint = configuration["GameEndpoint"];
            
            logger.LogWarning("Trying to connect cosmos endpoint: " + dbClient.Endpoint);

            var databaseResp = dbClient.CreateDatabaseIfNotExistsAsync("Puerto").Result;

            _actionsContainer = databaseResp.Database
                .CreateContainerIfNotExistsAsync("Actions", "/GameId")
                .Result.Container;

            _gamesContainer = databaseResp.Database
                .CreateContainerIfNotExistsAsync("Games", "/id")
                .Result.Container;
        }

        public async Task AddAction(string gameId, string playerId, IAction action) {
            var item = new ActionEntity {
                Id = Guid.NewGuid().ToString(),
                GameId = gameId,
                UserId = playerId,
                Action = action,
            };
            await _actionsContainer.CreateItemAsync(item, item.GetPartitionKey());
        }

        public async Task<IEnumerable<ActionEntity>> GetActionsByGame(string gameId) {
            var query = _actionsContainer.GetItemLinqQueryable<ActionEntity>()
                .Where(a => a.GameId == gameId)
                .OrderBy(a => a.TimeStamp)
                .ToFeedIterator();
            var results = await ExecuteQueryAsync(query);
            return results;
        }

        public Task<IEnumerable<GameEntity>> GetInProgressGamesForCurrentApplication() {
            return GetGames(g => g.Status == GameStatus.RUNNING && g.Endpoint == _gameEndpoint);
        }

        public Task<IEnumerable<GameEntity>> GetStartedGamesByPlayer(string userId) {
            return GetGames(g => g.Status != GameStatus.INITIAL && g.Players.Any(p => p.UserId == userId));
        }

        public Task<IEnumerable<GameEntity>> GetLobbyGames() {
            return GetGames(g => g.Status == GameStatus.INITIAL);
        }

        public async Task CreateGame(GameEntity gameEntity) {
            gameEntity.Endpoint = _gameEndpoint;
            await _gamesContainer.CreateItemAsync(gameEntity, gameEntity.GetPartitionKey());

            _logger.LogWarning("Game created: " + gameEntity.Name);
        }

        public async Task ReplaceGame(GameEntity gameEntity) {
            await _gamesContainer.ReplaceItemAsync(gameEntity, gameEntity.Id, gameEntity.GetPartitionKey());
        }

        public async Task DeleteGame(string gameId) {
            await _gamesContainer.DeleteItemAsync<GameEntity>(gameId, new PartitionKey(gameId));
            // action deletion is implemented as cosmos trigger & stored procedure
        }

        public async Task<GameEntity> GetGame(string gameId) {
            var result = await _gamesContainer.ReadItemAsync<GameEntity>(gameId, new PartitionKey(gameId));
            return result.Resource;
        }

        private async Task<IEnumerable<GameEntity>> GetGames(Expression<Func<GameEntity, bool>> predicate) {
            var query = _gamesContainer.GetItemLinqQueryable<GameEntity>()
                .Where(predicate)
                .ToFeedIterator();

            var results = await ExecuteQueryAsync(query);
            return results;
        }

        private async Task<IEnumerable<T>> ExecuteQueryAsync<T>(FeedIterator<T> query) {
            var results = new List<T>();
            while (query.HasMoreResults) {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }
    }
}