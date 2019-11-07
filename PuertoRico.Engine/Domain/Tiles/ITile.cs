using PuertoRico.Engine.Domain.Resources;

namespace PuertoRico.Engine.Domain.Tiles
{
    public interface ITile
    {
        string Name { get; }
        Colonist Worker { get; }
        void AddWorker(Colonist worker);
        Colonist RemoveWorker();
    }
}