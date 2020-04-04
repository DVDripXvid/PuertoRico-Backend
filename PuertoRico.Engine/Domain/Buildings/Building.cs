using System.Collections.Generic;
using System.Linq;
using PuertoRico.Engine.Domain.Resources;
using PuertoRico.Engine.Exceptions;

namespace PuertoRico.Engine.Domain.Buildings
{
    public abstract class Building : IBuilding
    {
        public abstract int Cost { get; }
        public abstract int VictoryPoint { get; }
        public List<Colonist> Workers { get; } = new List<Colonist>();
        public string Name => GetType().Name;
        public abstract int MaxDiscountByQuarry { get; }
        public abstract BuildingType Type { get; }
        public abstract int WorkerCapacity { get; }

        public void AddWorker(Colonist colonist) {
            if (WorkerCapacity == Workers.Count) {
                throw new GameException($"{Name} is full");
            }
            Workers.Add(colonist);
        }

        public Colonist RemoveWorker() {
            if (Workers.Count == 0) {
                throw new GameException($"No worker on {this.GetType().Name}");
            }
            var worker = Workers.First();
            Workers.Remove(worker);
            return worker;
        }

        public bool IsWorking() {
            return Workers.Count > 0;
        }
    }
}