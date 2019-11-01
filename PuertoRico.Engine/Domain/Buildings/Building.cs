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
        public List<IColonist> Workers { get; } = new List<IColonist>();
        public string Name => GetType().Name;
        public abstract int WorkerCapacity { get; }

        public void AddWorker(IColonist colonist) {
            if (WorkerCapacity == Workers.Count) {
                throw new GameException($"{Name} is full");
            }
            Workers.Add(colonist);
        }

        public IColonist RemoveWorker() {
            if (Workers.Count == 0) {
                throw new GameException($"No worker on {this.GetType().Name}");
            }
            var worker = Workers.First();
            Workers.Remove(worker);
            return worker;
        }
    }
}