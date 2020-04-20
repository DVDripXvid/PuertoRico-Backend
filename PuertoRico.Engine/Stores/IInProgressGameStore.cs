using PuertoRico.Engine.Domain;

namespace PuertoRico.Engine.Stores
{
    public interface IInProgressGameStore
    {
        Game FindById(string gameId);
        void Add(Game game);
        Game Remove(string gameId);
    }
}