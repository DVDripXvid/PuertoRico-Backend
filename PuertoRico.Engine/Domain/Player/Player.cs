using System.Collections.Generic;
using PuertoRico.Engine.Domain.Buildings;
using PuertoRico.Engine.Domain.Resources;
using PuertoRico.Engine.Domain.Tiles;

namespace PuertoRico.Engine.Domain.Player
{
    public class Player : IPlayer
    {
        public BuildingCollection Buildings { get; } = new BuildingCollection();
        public TilesCollection Tiles { get;  } = new TilesCollection();
        public IEnumerable<IColonist> Colonists => _colonists;
        public IEnumerable<IVictoryPointChip> VictoryPointChips => _victoryPointChips;

        private readonly List<IColonist> _colonists = new List<IColonist>();
        private readonly List<IVictoryPointChip> _victoryPointChips = new List<IVictoryPointChip>();

        public void Build(IBuilding building) {
            Buildings.Add(building);
        }

        public void Plant(ITile tile) {
            Tiles.Add(tile);
        }

        public void AddColonist(IColonist colonist) {
            _colonists.Add(colonist);
        }

        public void AddVictoryPointChips(IEnumerable<IVictoryPointChip> victoryPointChips) {
            _victoryPointChips.AddRange(victoryPointChips);
        }
    }
}