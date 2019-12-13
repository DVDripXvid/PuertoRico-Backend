using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using PuertoRico.Engine.Actions;

namespace PuertoRico.Engine.DAL
{
    public class CosmosGameRepository : IGameRepository
    {
        private readonly Container _actionsContainer;
        private readonly Container _gamesContainer;

        public CosmosGameRepository(CosmosClient dbClient) {
            var databaseResp = dbClient.CreateDatabaseIfNotExistsAsync("Puerto").Result;

            _actionsContainer = databaseResp.Database
                .CreateContainerIfNotExistsAsync("Actions", "/GameId")
                .Result.Container;

            _gamesContainer = databaseResp.Database
                .CreateContainerIfNotExistsAsync("Games", "/IsStarted")
                .Result.Container;
        }

        public async Task AddAction(string gameId, string playerId, IAction action) {
            var item = new ActionEntity {
                Id = Guid.NewGuid().ToString(),
                GameId = gameId,
                UserId = playerId,
                Action = action,
                SeqNumber = 21
            };
            await _actionsContainer.CreateItemAsync(item, item.GetPartitionKey());
        }

        public async Task<IEnumerable<ActionEntity>> GetActionsByGame(string gameId) {
            var query = _actionsContainer.GetItemLinqQueryable<ActionEntity>()
                .Where(a => a.GameId == gameId)
                .ToFeedIterator();
            /*var queryString = $"SELECT * FROM Actions a WHERE a.GameId = \"{gameId}\"";
            var query = _actionsContainer.GetItemQueryIterator<ActionEntity>(new QueryDefinition(queryString));*/
            var results = await ExecuteQueryAsync(query);
            return results;
        }

        public Task<IEnumerable<GameEntity>> GetStartedGames() {
            return GetGames(g => g.IsStarted);
        }

        public Task<IEnumerable<GameEntity>> GetLobbyGames() {
            return GetGames(g => !g.IsStarted);
        }

        public async Task CreateGame(GameEntity gameEntity) {
            await _gamesContainer.CreateItemAsync(gameEntity, gameEntity.GetPartitionKey());
        }

        public async Task ReplaceGame(GameEntity gameEntity) {
            await _gamesContainer.ReplaceItemAsync(gameEntity, gameEntity.Id, gameEntity.GetPartitionKey());
        }

        public async Task DeleteGame(string gameId, bool isStarted) {
            await _gamesContainer.DeleteItemAsync<GameEntity>(gameId, new PartitionKey(isStarted));
            // action deletion is implemented as cosmos trigger & stored procedure
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