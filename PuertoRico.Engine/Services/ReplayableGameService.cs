using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.DAL;
using PuertoRico.Engine.Domain;
using PuertoRico.Engine.DTOs;

namespace PuertoRico.Engine.Services
{
    public class ReplayableGameService : IReplayableGameService
    {
        private readonly IGameService _gameService;
        private readonly IGameRepository _repository;

        public ReplayableGameService(IGameRepository repository, IGameService gameService)
        {
            _repository = repository;
            _gameService = gameService;
        }

        public async Task ExecuteRoleAction(Game game, string userId, IAction action)
        {
            await _gameService.ExecuteRoleAction(game, userId, action);
            await _repository.AddAction(game.Id, userId, action);
        }

        public async Task ExecuteSelectRole(Game game, string userId, SelectRole selectRole)
        {
            await _gameService.ExecuteSelectRole(game, userId, selectRole);
            await _repository.AddAction(game.Id, userId, selectRole);
        }

        public Task<IEnumerable<ActionType>> GetAvailableActionTypeForUser(Game game, string userId)
        {
            return _gameService.GetAvailableActionTypeForUser(game, userId);
        }

        public async Task<IEnumerable<PlayerResultDto>> HandleFinishedGame(Game game)
        {
            var results = game.Players.Select(p => new PlayerResultDto
            {
                Player = PlayerDto.Create(p),
                Result = p.CalculateVictoryPoints()
            }).ToList();

            var entity = GameEntity.Create(game);
            foreach (var player in entity.Players)
            {
                player.Result = results.First(r => r.Player.UserId == player.UserId).Result;
            }
            // TODO: use redis ttl
            // entity.ttl = 60 * 60 * 24 * 7;  // Delete finished in a week
            await _repository.ReplaceGame(entity);

            return results;
        }
    }
}