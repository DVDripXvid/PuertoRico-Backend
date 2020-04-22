using System.Collections.Generic;
using System.Threading.Tasks;
using PuertoRico.Engine.Actions;

namespace PuertoRico.Engine.DAL
{
    public interface IGameRepository
    {
        Task AddAction(string gameId, string playerId, IAction action);
        Task<IEnumerable<ActionEntity>> GetActionsByGame(string gameId);
        Task<IEnumerable<GameEntity>> GetInProgressGamesForCurrentApplication();
        Task<IEnumerable<GameEntity>> GetStartedGamesByPlayer(string userId);
        Task<IEnumerable<GameEntity>> GetLobbyGames();
        Task CreateGame(GameEntity gameEntity);
        Task ReplaceGame(GameEntity gameEntity);
        Task DeleteGame(string gameId);
        Task<GameEntity> GetGame(string gameId);
    }
}