using System;
using System.Collections.Generic;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain.Buildings;
using PuertoRico.Engine.Domain.Resources;
using PuertoRico.Engine.Domain.Resources.Goods;
using PuertoRico.Engine.Domain.Roles;
using PuertoRico.Engine.Domain.Tiles;

namespace PuertoRico.Engine.Domain.Player
{
    public class Player : IPlayer
    {
        public BuildingCollection Buildings { get; } = new BuildingCollection();
        public TileCollection Tiles { get; } = new TileCollection();
        public List<Colonist> IdleColonists { get; } = new List<Colonist>();
        public List<VictoryPointChip> VictoryPointChips { get; } = new List<VictoryPointChip>();
        public IRole Role { get; private set; }
        public int Doubloons { get; set; }
        public List<IGood> Goods { get; } = new List<IGood>();
        public string UserId { get; }

        [Obsolete]
        public Player() {
            UserId = Guid.NewGuid().ToString();
        }

        public Player(string userId) {
            UserId = userId;
        }

        public void Build(IBuilding building) {
            Buildings.Add(building);
        }

        public void Plant(ITile tile) {
            Tiles.Add(tile);
        }

        public void AddColonist(Colonist colonist) {
            IdleColonists.Add(colonist);
        }

        public void AddVictoryPointChips(IEnumerable<VictoryPointChip> victoryPointChips) {
            VictoryPointChips.AddRange(victoryPointChips);
        }

        public void AddGoods(IEnumerable<IGood> goods) {
            Goods.AddRange(goods);
        }

        public void SelectRole(IRole role, Game game) {
            if (Role != null) {
                throw new InvalidOperationException("Cannot select a new role before putting the current back");
            }
            game.Roles.Remove(role);
            role.OnSelect(this);
            Role = role;
        }

        public void PutBackRole(Game game) {
            if (Role != null) {
                throw new InvalidOperationException("Player has no role");
            }
            game.Roles.Add(Role);
            Role = null;
        }
    }
}