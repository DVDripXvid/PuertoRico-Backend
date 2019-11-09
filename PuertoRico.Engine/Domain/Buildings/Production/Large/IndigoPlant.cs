using PuertoRico.Engine.Domain.Resources.Goods;

namespace PuertoRico.Engine.Domain.Buildings.Production.Large
{
    public class IndigoPlant : LargeProductionBuilding<Indigo>
    {
        public override int Cost => 3;
        public override int VictoryPoint => 2;
        public override int MaxDiscountByQuarry => 2;
    }
}