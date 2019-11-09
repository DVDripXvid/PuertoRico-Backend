using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PuertoRico.Engine.Domain.Buildings.Large;
using PuertoRico.Engine.Domain.Buildings.Production;
using PuertoRico.Engine.Domain.Buildings.Production.Large;
using PuertoRico.Engine.Domain.Buildings.Production.Small;
using PuertoRico.Engine.Domain.Buildings.Small;
using PuertoRico.Engine.Domain.Resources.Goods;
using PuertoRico.Engine.Exceptions;

namespace PuertoRico.Engine.Domain.Buildings
{
    public class BuildingCollection : IEnumerable<IBuilding>
    {
        private readonly List<IBuilding> _buildings = new List<IBuilding>(12);

        public IEnumerable<ISmallProductionBuilding> SmallProductionBuildings =>
            _buildings.OfType<ISmallProductionBuilding>();

        public IEnumerable<ILargeProductionBuilding> LargeProductionBuildings =>
            _buildings.OfType<ILargeProductionBuilding>();

        public IEnumerable<IProductionBuilding> ProductionBuildings =>
            _buildings.OfType<IProductionBuilding>();

        public IEnumerable<LargeBuilding> LargeBuildings => _buildings.OfType<LargeBuilding>();
        public IEnumerable<SmallBuilding> SmallBuildings => _buildings.OfType<SmallBuilding>();

        public IBuilding this[int index] => _buildings[index];

        public void Add(IBuilding building) {
            if (!CanBeAdded(building)) {
                throw new GameException($"{building.Name} cannot be built");
            }

            _buildings.Add(building);
        }

        public bool IsFull() {
            var filledSlots = ComputeFilledSlots();
            return filledSlots == 12;
        }

        public bool CanBeAdded(IBuilding building) {
            if (_buildings.Exists(b => b.GetType() == building.GetType())) {
                return false;
            }

            if (building is LargeBuilding) {
                var filledSlots = ComputeFilledSlots();
                return filledSlots <= 10;
            }

            return !IsFull();
        }

        public bool ContainsWorkingOfType<T>() where T : IBuilding {
            var building = _buildings.OfType<T>().FirstOrDefault();
            return building != null && building.IsWorking();
        }

        public IEnumerator<IBuilding> GetEnumerator() {
            return _buildings.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        private int ComputeFilledSlots() {
            return ProductionBuildings.Count()
                   + SmallBuildings.Count()
                   + LargeBuildings.Count() * 2;
        }
    }
}