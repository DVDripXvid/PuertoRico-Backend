using PuertoRico.Engine.Domain.Resources.Goods;

namespace PuertoRico.Engine.Domain.Buildings.Production.Large
{
    public class CoffeeRoaster : LargeProductionBuilding<Coffee>
    {
        public override int Cost => 6;
        public override int VictoryPoint => 3;
        public override int MaxDiscountByQuarry => 3;
        public override int WorkerCapacity => 2;
    }
}