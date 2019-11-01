using System.Collections.Generic;
using PuertoRico.Engine.Domain.Resources;

namespace PuertoRico.Engine.Domain.Buildings
{
    public interface IBuilding
    {
        int Cost { get; }
        int VictoryPoint { get; }
        List<IColonist> Workers { get; }
        int WorkerCapacity { get; }
        string Name { get; }

        void AddWorker(IColonist colonist);
        IColonist RemoveWorker();
    }
}