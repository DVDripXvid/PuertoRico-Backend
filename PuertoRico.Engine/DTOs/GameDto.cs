using System.Collections.Generic;
using System.Linq;
using PuertoRico.Engine.Domain;
using PuertoRico.Engine.Domain.Resources.Goods;

namespace PuertoRico.Engine.DTOs
{
    public class GameDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ICollection<TileDto> VisiblePlantations { get; set; }
        public ICollection<BuildingDto> Buildings { get; set; }
        public int ColonistsCount { get; set; }
        public int QuarryCount { get; set; }
        public ICollection<PlayerDto> Players { get; set; }
        public PlayerDto CurrentPlayer { get; set; }
        public RoleDto CurrentRole { get; set; }
        public ColonistsShipDto ColonistsShip { get; set; }
        public TradeHouseDto TradeHouse { get; set; }
        public int CornCount { get; set; }
        public int IndigoCount { get; set; }
        public int SugarCount { get; set; }
        public int TobaccoCount { get; set; }
        public int CoffeeCount { get; set; }
        public ICollection<CargoShipDto> CargoShips { get; set; }
        public int VictoryPoints { get; set; }
        public ICollection<RoleDto> SelectableRoles { get; set; }

        public GameDto(Game game) {
            Id = game.Id;
            Name = game.Name;
            VisiblePlantations = game.PlantationDeck.Drawable
                .Select((p, idx) => new TileDto(p, idx))
                .ToList();
            Buildings = game.Buildings
                .Select((b, idx) => new BuildingDto(b, idx))
                .ToList();
            ColonistsCount = game.Colonists.Count;
            QuarryCount = game.Quarries.Count;
            //TODO: set username
            Players = game.Players
                .Select(p => new PlayerDto(p.UserId, p))
                .ToList();
            var player = game.CurrentRoleOwnerPlayer.Role == null
                ? game.CurrentRoleOwnerPlayer
                : game.CurrentRoleOwnerPlayer.Role.CurrentPlayer;
            CurrentPlayer = new PlayerDto(player.UserId, player);
            CurrentRole = new RoleDto(game.CurrentRoleOwnerPlayer.Role, -1);
            ColonistsShip = new ColonistsShipDto(game.ColonistsShip);
            TradeHouse = new TradeHouseDto(game.TradeHouse);
            CornCount = game.Goods.Count(g => g.Type == GoodType.Corn);
            IndigoCount = game.Goods.Count(g => g.Type == GoodType.Indigo);
            SugarCount = game.Goods.Count(g => g.Type == GoodType.Sugar);
            TobaccoCount = game.Goods.Count(g => g.Type == GoodType.Tobacco);
            CoffeeCount = game.Goods.Count(g => g.Type == GoodType.Coffee);
            CargoShips = game.CargoShips
                .Select(cs => new CargoShipDto(cs))
                .ToList();
            VictoryPoints = game.VictoryPointChips.Count;
            SelectableRoles = game.Roles
                .Select((r, idx) => new RoleDto(r, idx))
                .ToList();
        }
    }
}