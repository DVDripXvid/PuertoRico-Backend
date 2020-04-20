using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
    public class InMemoryInProgressGameStore : IInProgressGameStore
    {
        private readonly Dictionary<string, Game> _games = new Dictionary<string, Game>();
        private readonly IGameService _gameService;
        private readonly ILogger<InMemoryInProgressGameStore> _logger;

        public InMemoryInProgressGameStore(IGameRepository repository, IGameService gameService,
            ILogger<InMemoryInProgressGameStore> logger) {
            _gameService = gameService;
            _logger = logger;
            InitializeGamesFromDb(repository).Wait();
        }

        public Game FindById(string gameId) {
            if (_games.ContainsKey(gameId)) {
                return _games[gameId];
            }
            
            throw new GameException("Game not found");
        }

        public void  Add(Game game) {
            if (_games.ContainsKey(game.Id)) {
                throw new InvalidOperationException("Game already added with id = " + game.Id);
            }
            
            _games[game.Id] = game;
        }

        public Game Remove(string gameId) {
            if (_games.TryGetValue(gameId, out var game)) {
                _games.Remove(gameId);
                return game;
            }

            throw new InvalidOperationException($"Game with id = {gameId} does not exist");
        }
        
        private async Task InitializeGamesFromDb(IGameRepository repository) {
            var games = await repository.GetStartedGamesForCurrentApplication();
            foreach (var gameEntity in games) {
                var game = new Game(gameEntity.Id, gameEntity.Name, gameEntity.RandomSeed);
                gameEntity.Players.ToList().ForEach(p => game.Join(new Player(p.UserId, p.Username, p.PictureUrl)));
                game.Start();
                var actions = await repository.GetActionsByGame(game.Id);
                var isValidGame = true;
                foreach (var a in actions.ToList()) {
                    try {
                        if (a.Action is SelectRole selectRole) {
                            await _gameService.ExecuteSelectRole(game, a.UserId, selectRole);
                        }
                        else {
                            await _gameService.ExecuteRoleAction(game, a.UserId, a.Action);
                        }
                    }
                    catch (Exception e) {
                        _logger.LogError(e, $"Failed to replay {a.Action.ActionType} action. gameId={game.Id} userId={a.UserId}, timestamp={a.TimeStamp}");
                        _logger.LogError($"Failed to initialize game: ${game.Name} (${game.Id})");
                        isValidGame = false;
                        break;
                    }
                }

                if (isValidGame) _games.Add(game.Id, game);
            }
        }
    }
}