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
        public string Endpoint { get; set; }

        public static GameDto Create(Game game) {
            var currentPlayer = game.GetCurrentPlayer();
            return new GameDto {
                Id = game.Id,
                Name = game.Name,
                VisiblePlantations = game.PlantationDeck.Drawable
                    .Select(TileDto.Create)
                    .ToList(),
                Buildings = game.Buildings
                    .Select(BuildingDto.Create)
                    .ToList(),
                ColonistsCount = game.Colonists.Count,
                QuarryCount = game.Quarries.Count,
                Players = game.Players.Select(PlayerDto.Create).ToList(),
                CurrentPlayer = PlayerDto.Create(currentPlayer),
                CurrentRole = RoleDto.Create(game.CurrentRoleOwnerPlayer.Role, -1),
                ColonistsShip = ColonistsShipDto.Create(game.ColonistsShip),
                TradeHouse = TradeHouseDto.Create(game.TradeHouse),
                CornCount = game.Goods.Count(g => g.Type == GoodType.Corn),
                IndigoCount = game.Goods.Count(g => g.Type == GoodType.Indigo),
                SugarCount = game.Goods.Count(g => g.Type == GoodType.Sugar),
                TobaccoCount = game.Goods.Count(g => g.Type == GoodType.Tobacco),
                CoffeeCount = game.Goods.Count(g => g.Type == GoodType.Coffee),
                CargoShips = game.CargoShips
                    .Select(CargoShipDto.Create)
                    .ToList(),
                VictoryPoints = game.VictoryPointChips.Count,
                SelectableRoles = game.Roles
                    .Select(RoleDto.Create)
                    .ToList(),
                Endpoint = game.Endpoint
            };
        }
    }
}