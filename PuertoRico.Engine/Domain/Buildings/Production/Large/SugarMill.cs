using PuertoRico.Engine.Domain.Resources.Goods;

namespace PuertoRico.Engine.Domain.Buildings.Production.Large
{
    public class SugarMill : LargeProductionBuilding<Sugar>
    {
        public override int Cost => 4;
        public override int VictoryPoint => 2;
        public override int MaxDiscountByQuarry => 2;
    }
}