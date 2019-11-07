namespace PuertoRico.Engine.Domain.Buildings.Small
{
    public class University : SmallBuilding
    {
        public override int Cost => 8;
        public override int VictoryPoint => 3;
        public override int MaxDiscountByQuarry => 3;
    }
}