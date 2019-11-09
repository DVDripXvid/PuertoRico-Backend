using PuertoRico.Engine.Domain.Resources.Goods;

namespace PuertoRico.Engine.Domain.Buildings.Production.Large
{
    public class TobaccoStorage : LargeProductionBuilding<Tobacco>
    {
        public override int Cost => 5;
        public override int VictoryPoint => 3;
        public override int MaxDiscountByQuarry => 3;
    }
}