using System.Linq;
using PuertoRico.Engine.Domain.Player;

namespace PuertoRico.Engine.Domain.Buildings.Large
{
    public class Fortress : LargeBuilding
    {
        public override int ComputeVictoryPoints(IPlayer player) {
            return (player.IdleColonists.Count()
                    + player.Buildings.Select(b => b.Workers.Count()).Sum())
                   / 3;
        }
    }
}