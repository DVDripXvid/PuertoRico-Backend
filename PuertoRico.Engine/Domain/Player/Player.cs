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
        public List<IColonist> Colonists { get; } = new List<IColonist>();
        public List<IVictoryPointChip> VictoryPointChips { get; } = new List<IVictoryPointChip>();

        public void Build(IBuilding building) {
            Buildings.Add(building);
        }

        public void Plant(ITile tile) {
            Tiles.Add(tile);
        }

        public void AddColonist(IColonist colonist) {
            Colonists.Add(colonist);
        }

        public void AddVictoryPointChips(IEnumerable<IVictoryPointChip> victoryPointChips) {
            VictoryPointChips.AddRange(victoryPointChips);
        }
    }
}