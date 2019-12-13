using System.Collections.Generic;
using System.Threading.Tasks;
using PuertoRico.Engine.Domain;
using PuertoRico.Engine.Domain.Player;

namespace PuertoRico.Engine.Stores
{
    public interface IGameStore
    {
        Game FindById(string gameId);
        IEnumerable<Game> FindNotStarted();
        Task Add(Game game);
        Task<Game> Remove(string gameId);
        IEnumerable<Game> FindByUserId(string userId);
        Task JoinGame(string gameId, IPlayer player);
        Task<IPlayer> LeaveGame(string gameId, string userId);
    }
}