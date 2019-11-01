using System.Linq;
using PuertoRico.Engine.Domain.Player;

namespace PuertoRico.Engine.Domain.Buildings.Large
{
    public class Fortress : LargeBuilding
    {
        public override int ComputeVictoryPoints(IPlayer player) {
            return player.Colonists.Count() / 3;
        }
    }
}