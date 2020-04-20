using System.Collections.Generic;
using System.Linq;
using PuertoRico.Engine.Domain;

namespace PuertoRico.Engine.DTOs
{
    public class GameSummaryDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Endpoint { get; set; }
        public ICollection<PlayerDto> Players { get; set; }

        public static GameSummaryDto Create(Game game) {
            return new GameSummaryDto {
                Id = game.Id,
                Name = game.Name,
                Endpoint = game.Endpoint,
                Players = game.Players.Select(PlayerDto.Create).ToList()
            };
        }
    }
}