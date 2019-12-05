using System.Collections.Generic;
using PuertoRico.Engine.Domain;

namespace PuertoRico.Engine.Stores
{
    public interface IGameStore
    {
        Game FindById(string gameId);
        IEnumerable<Game> FindNotStarted();
        void Add(Game game);
        void Remove(string gameId);
        IEnumerable<Game> FindByUserId(string userId);
    }
}