using PuertoRico.Engine.Domain.Resources;
using PuertoRico.Engine.Exceptions;

namespace PuertoRico.Engine.Domain.Tiles
{
    public abstract class Tile : ITile
    {
        public string Name => GetType().Name;

        public Colonist Worker { get; private set; }

        public void AddWorker(Colonist worker) {
            if (Worker != null) {
                throw new GameException($"{Name} already has a worker");
            }

            Worker = worker;
        }

        public Colonist RemoveWorker() {
            if (Worker == null) {
                throw new GameException($"{Name} already has no worker");
            }

            var worker = Worker;
            Worker = null;
            return worker;
        }
    }
}