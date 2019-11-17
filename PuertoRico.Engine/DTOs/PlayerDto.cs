using System.Collections.Generic;
using System.Linq;
using PuertoRico.Engine.Domain.Player;

namespace PuertoRico.Engine.DTOs
{
    public class PlayerDto
    {
        public string UserName { get; set; }
        public string UserId { get; set; }
        public ICollection<BuildingDto> Buildings { get; set; }
        public ICollection<TileDto> Tiles { get; set; }
        public int IdleColonistCount { get; set; }
        public int VictoryPoints { get; set; }
        public RoleDto Role { get; set; }
        public ICollection<GoodDto> Goods { get; set; }
        public int Doubloons { get; set; }

        public static PlayerDto Create(IPlayer player) {
            return new PlayerDto {
                UserName = player.Username,
                UserId = player.UserId,
                Buildings = player.Buildings
                    .Select(BuildingDto.Create)
                    .ToList(),
                Tiles = player.Tiles
                    .Select(TileDto.Create)
                    .ToList(),
                IdleColonistCount = player.IdleColonists.Count,
                VictoryPoints = player.VictoryPointChips.Count,
                Role = RoleDto.Create(player.Role, -1),
                Goods = player.Goods
                    .Select(g => GoodDto.Create(g.Type))
                    .ToList(),
                Doubloons = player.Doubloons,
            };
        }
    }
}