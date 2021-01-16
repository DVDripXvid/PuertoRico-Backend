using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PuertoRico.Engine.DAL.Redis
{
    public class RedisGameRepository : IGameRepository
    {
        private readonly ConnectionMultiplexer _redisConnection;
        private readonly IDatabase _redisDb;
        private readonly string _gameEndpoint;
        private readonly ILogger<RedisGameRepository> _logger;

        public RedisGameRepository(ConnectionMultiplexer redisConnection, IConfiguration configuration, ILogger<RedisGameRepository> logger)
        {
            _redisConnection = redisConnection;
            _redisDb = redisConnection.GetDatabase();
            _gameEndpoint = configuration["GameEndpoint"];
            _logger = logger;
        }

        public async Task AddAction(string gameId, string playerId, IAction action)
        {
            var item = new ActionEntity
            {
                GameId = gameId,
                UserId = playerId,
                Action = action,
            };
            var value = JsonConvert.SerializeObject(item);
            await _redisDb.ListRightPushAsync(gameId, value);
        }

        public async Task CreateGame(GameEntity gameEntity)
        {
            await _redisDb.KeyDeleteAsync(gameEntity.Id);
            gameEntity.Endpoint = _gameEndpoint;
            var value = JsonConvert.SerializeObject(gameEntity);
            await _redisDb.ListRightPushAsync(gameEntity.Id, value);
            _logger.LogInformation("Game created: {} ({})", gameEntity.Name, gameEntity.Id);
        }

        public async Task DeleteGame(string gameId)
        {
            await _redisDb.KeyDeleteAsync(gameId);
        }

        public async Task<IEnumerable<ActionEntity>> GetActionsByGame(string gameId)
        {
            var items = await _redisDb.ListRangeAsync(gameId, 1);
            return items.ToStringArray()
                .Skip(1)
                .Select(item => JsonConvert.DeserializeObject<ActionEntity>(item));
        }

        public async Task<GameEntity> GetGame(string gameId)
        {
            var item = await _redisDb.ListGetByIndexAsync(gameId, 0);
            return JsonConvert.DeserializeObject<GameEntity>(item);
        }

        public Task<IEnumerable<GameEntity>> GetInProgressGamesForCurrentApplication()
        {
            return GetAllGames(g => g.Status == GameStatus.RUNNING && g.Endpoint == _gameEndpoint);
        }

        public Task<IEnumerable<GameEntity>> GetStartedGamesByPlayer(string userId)
        {
            return GetAllGames(g => g.Status != GameStatus.INITIAL && g.Players.Any(p => p.UserId == userId));
        }

        public Task<IEnumerable<GameEntity>> GetLobbyGames()
        {
            return GetAllGames(g => g.Status == GameStatus.INITIAL);
        }

        public Task ReplaceGame(GameEntity gameEntity)
        {
            throw new NotImplementedException();
        }

        private async Task<IEnumerable<GameEntity>> GetAllGames(Func<GameEntity, bool> predicate)
        {
            var results = new List<GameEntity>();
            foreach (var endpoint in _redisConnection.GetEndPoints())
            {
                var server = _redisConnection.GetServer(endpoint);
                await foreach (var key in server.KeysAsync())
                {
                    var game = await GetGame(key);
                    if (predicate(game))
                    {
                        results.Add(game);
                    }
                }
            }
            return results;
        }
    }
}