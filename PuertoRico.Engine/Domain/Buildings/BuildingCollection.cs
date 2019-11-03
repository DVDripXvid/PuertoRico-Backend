using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PuertoRico.Engine.Domain.Buildings.Large;
using PuertoRico.Engine.Domain.Buildings.Production;
using PuertoRico.Engine.Domain.Buildings.Production.Large;
using PuertoRico.Engine.Domain.Buildings.Production.Small;
using PuertoRico.Engine.Domain.Buildings.Small;

namespace PuertoRico.Engine.Domain.Buildings
{
    public class BuildingCollection : IEnumerable<IBuilding>
    {
        private readonly List<IBuilding> _buildings;

        public IEnumerable<SmallProductionBuilding> SmallProductionBuildings => _buildings.OfType<SmallProductionBuilding>();
        public IEnumerable<LargeProductionBuilding> LargeProductionBuildings => _buildings.OfType<LargeProductionBuilding>();
        public IEnumerable<ProductionBuilding> ProductionBuildings => _buildings.OfType<ProductionBuilding>();
        public IEnumerable<LargeBuilding> LargeBuildings => _buildings.OfType<LargeBuilding>();
        public IEnumerable<SmallBuilding> SmallBuildings => _buildings.OfType<SmallBuilding>();

        public BuildingCollection() {
            _buildings =  new List<IBuilding>();
        }

        public void Add(IBuilding building) {
            _buildings.Add(building);
        }

        public IEnumerator<IBuilding> GetEnumerator() {
            return _buildings.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}