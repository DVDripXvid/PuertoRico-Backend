using System.Linq;
using PuertoRico.Engine.Domain.Player;

namespace PuertoRico.Engine.Domain.Buildings.Large
{
    public class GuildHall : LargeBuilding
    {
        public override int ComputeVictoryPoints(IPlayer player) {
            return player.Buildings.SmallProductionBuildings.Count()
                   + player.Buildings.LargeProductionBuildings.Count() * 2;
        }
    }
}