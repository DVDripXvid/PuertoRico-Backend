using System.Linq;
using PuertoRico.Engine.Domain.Player;

namespace PuertoRico.Engine.Domain.Buildings.Large
{
    public class CityHall : LargeBuilding
    {
        public override int ComputeVictoryPoints(IPlayer player) {
            return player.Buildings.SmallBuildings.Count()
                + player.Buildings.LargeBuildings.Count();
        }
    }
}