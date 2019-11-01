using System.Linq;
using PuertoRico.Engine.Domain.Player;

namespace PuertoRico.Engine.Domain.Buildings.Large
{
    public class Residence : LargeBuilding
    {
        public override int ComputeVictoryPoints(IPlayer player) {
            var tilesCount = player.Tiles.Count();
            return tilesCount <= 9
                ? 4
                : tilesCount - 5;
        }
    }
}