using System.Collections.Generic;
using PuertoRico.Engine.Domain.Resources;

namespace PuertoRico.Engine.Domain.Buildings
{
    public interface IBuilding
    {
        int Cost { get; }
        int VictoryPoint { get; }
        List<Colonist> Workers { get; }
        int WorkerCapacity { get; }
        string Name { get; }
        BuildingType Type { get; }
        int MaxDiscountByQuarry { get; }

        void AddWorker(Colonist colonist);
        Colonist RemoveWorker();
        bool IsWorking();
    }
}