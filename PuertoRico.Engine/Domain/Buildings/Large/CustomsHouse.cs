using System.Linq;
using PuertoRico.Engine.Domain.Player;

namespace PuertoRico.Engine.Domain.Buildings.Large
{
    public class CustomsHouse : LargeBuilding
    {
        public override int ComputeVictoryPoints(IPlayer player) {
            return player.VictoryPointChips.Count() / 4;
        }
    }
}