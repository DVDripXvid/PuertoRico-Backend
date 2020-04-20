using System;
using System.Collections.Generic;
using System.Linq;
using PuertoRico.Engine.Domain.Resources;

namespace PuertoRico.Engine.Domain.Misc
{
    public class ColonistShip
    {
        public int ColonistCount => _colonists.Count;
        
        private int _capacity;
        private readonly Stack<Colonist> _colonists;

        public ColonistShip(Game game) {
            _capacity = game.PlayerCount;
            _colonists = new Stack<Colonist>(_capacity);
            Refill(game);
        }

        public void RecalculateCapacityAndRefill(Game game) {
            var sumOfAvailableBuildingSlots = game.Players
                .SelectMany(p => p.Buildings)
                .Select(b => b.WorkerCapacity - b.Workers.Count)
                .Sum();
            _capacity =  Math.Max(game.PlayerCount, sumOfAvailableBuildingSlots);
            Refill(game);
        }

        public Colonist TakeColonist() {
            return _colonists.Pop();
        }

        public bool IsEmpty() {
            return _colonists.Count == 0;
        }
        
        private void Refill(Game game) {
            while (_colonists.Count < _capacity) {
                if (game.Colonists.Count == 0) {
                    game.SendShouldFinishSignal();
                    break;
                }
                _colonists.Push(game.Colonists.Pop());
            }
        }
    }
}