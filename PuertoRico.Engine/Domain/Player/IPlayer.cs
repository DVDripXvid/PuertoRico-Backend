using System.Collections;
using System.Collections.Generic;
using PuertoRico.Engine.Domain.Buildings;
using PuertoRico.Engine.Domain.Resources;
using PuertoRico.Engine.Domain.Tiles;

namespace PuertoRico.Engine.Domain.Player
{
    public interface IPlayer
    {
        BuildingCollection Buildings { get; }
        TilesCollection Tiles { get; }
        IEnumerable<IColonist> Colonists { get; }

        IEnumerable<IVictoryPointChip> VictoryPointChips { get; }

        void Build(IBuilding building);

        void Plant(ITile tile);

        void AddColonist(IColonist colonist);

        void AddVictoryPointChips(IEnumerable<IVictoryPointChip> victoryPointChips);
    }
}