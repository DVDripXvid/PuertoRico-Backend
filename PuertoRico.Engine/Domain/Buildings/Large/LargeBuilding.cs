using PuertoRico.Engine.Domain.Player;

namespace PuertoRico.Engine.Domain.Buildings.Large
{
    public abstract class LargeBuilding : Building
    {
        public override int Cost => 10;
        public override int VictoryPoint => 4;
        public override int WorkerCapacity => 1;
        public override BuildingType Type => BuildingType.Large;
        public override int MaxDiscountByQuarry => 4;

        public abstract int ComputeVictoryPoints(IPlayer player);

        public int ComputeVictoryPointsIfWorking(IPlayer player) {
            return Workers.Count == 0
                ? 0
                : ComputeVictoryPoints(player);
        }
    }
}