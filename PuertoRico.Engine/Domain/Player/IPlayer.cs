using System.Collections;
using System.Collections.Generic;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain.Buildings;
using PuertoRico.Engine.Domain.Resources;
using PuertoRico.Engine.Domain.Resources.Goods;
using PuertoRico.Engine.Domain.Roles;
using PuertoRico.Engine.Domain.Tiles;

namespace PuertoRico.Engine.Domain.Player
{
    public interface IPlayer
    {
        BuildingCollection Buildings { get; }
        TileCollection Tiles { get; }
        List<Colonist> IdleColonists { get; }
        List<VictoryPointChip> VictoryPointChips { get; }
        IRole Role { get; }
        List<IGood> Goods { get; }
        int Doubloons { get; set; }
        string UserId { get; }

        void Build(IBuilding building);

        void Plant(ITile tile);

        void AddColonist(Colonist colonist);

        void AddVictoryPointChips(IEnumerable<VictoryPointChip> victoryPointChips);

        void AddGoods(IEnumerable<IGood> goods);
        HashSet<ActionType> GetAvailableActionTypes();
        void SelectRole(IRole role);
    }
}