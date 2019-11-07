using System.Collections.Generic;
using System.Linq;
using PuertoRico.Engine.Domain.Resources;

namespace PuertoRico.Engine.Domain.Misc
{
    public class ColonistsShip
    {
        public int Capacity { get; private set; }

        private readonly Stack<Colonist> _colonists;

        public ColonistsShip(int capacity) {
            Capacity = capacity;
            _colonists = new Stack<Colonist>(capacity);
        }

        public void RecalculateCapacityAndRefill(Game game) {
            var sumOfAvailableBuildingSlots = game.Players
                .SelectMany(p => p.Buildings)
                .Select(b => b.WorkerCapacity - b.Workers.Count)
                .Sum();
            Capacity = sumOfAvailableBuildingSlots;
            Refill(game);
        }

        public Colonist TakeColonist() {
            return _colonists.Pop();
        }

        public bool IsEmpty() {
            return _colonists.Count == 0;
        }
        
        private void Refill(Game game) {
            while (_colonists.Count < Capacity) {
                _colonists.Push(game.Colonists.Pop());
            }
        }
    }
}