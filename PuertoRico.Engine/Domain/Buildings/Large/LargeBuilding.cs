using PuertoRico.Engine.Domain.Player;

namespace PuertoRico.Engine.Domain.Buildings.Large
{
    public abstract class LargeBuilding : Building
    {
        public override int Cost => 10;
        public override int VictoryPoint => 4;
        public override int WorkerCapacity => 1;

        public abstract int ComputeVictoryPoints(IPlayer player);
    }
}