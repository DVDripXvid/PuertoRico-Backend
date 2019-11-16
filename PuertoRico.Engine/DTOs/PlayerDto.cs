﻿using System.Collections.Generic;
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

        public PlayerDto(string userName, IPlayer player) {
            UserName = userName;
            UserId = player.UserId;
            Buildings = player.Buildings
                .Select((b, idx) => new BuildingDto(b, idx))
                .ToList();
            Tiles = player.Tiles
                .Select((t, idx) => new TileDto(t, idx))
                .ToList();
            IdleColonistCount = player.IdleColonists.Count;
            VictoryPoints = player.VictoryPointChips.Count;
            Role = new RoleDto(player.Role, -1);
            Goods = player.Goods
                .Select(g => new GoodDto(g.Type))
                .ToList();
            Doubloons = player.Doubloons;
        }
    }
}