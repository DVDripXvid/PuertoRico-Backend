using System;
using System.Collections.Generic;
using System.Linq;
using PuertoRico.Engine.Domain.Resources;

namespace PuertoRico.Engine.Domain.Misc
{
    public class ColonistsShip
    {
        public int Capacity { get; private set; }

        private readonly Stack<Colonist> _colonists;

        public ColonistsShip(Game game) {
            Capacity = game.PlayerCount;
            _colonists = new Stack<Colonist>(Capacity);
            Refill(game);
        }

        public void RecalculateCapacityAndRefill(Game game) {
            var sumOfAvailableBuildingSlots = game.Players
                .SelectMany(p => p.Buildings)
                .Select(b => b.WorkerCapacity - b.Workers.Count)
                .Sum();
            Capacity =  Math.Max(game.PlayerCount, sumOfAvailableBuildingSlots);
            Refill(game);
        }

        public Colonist TakeColonist() {
            return _colonists.Pop();
        }

        public bool IsEmpty() {
            return _colonists.Count == 0;
        }
        
        private void Refill(Game game) {
            while (_colonists.Count < Capacity && game.Colonists.Count > 0) {
                _colonists.Push(game.Colonists.Pop());
            }
        }
    }
}