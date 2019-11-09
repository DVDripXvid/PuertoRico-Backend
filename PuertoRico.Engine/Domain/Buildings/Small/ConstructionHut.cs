namespace PuertoRico.Engine.Domain.Buildings.Small
{
    public class ConstructionHut : SmallBuilding
    {
        public override int Cost => 2;
        public override int VictoryPoint => 1;
        public override int MaxDiscountByQuarry => 1;
    }
}