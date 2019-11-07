namespace PuertoRico.Engine.Domain.Buildings.Small
{
    public class Office : SmallBuilding
    {
        public override int Cost => 5;
        public override int VictoryPoint => 2;
        public override int MaxDiscountByQuarry => 2;
    }
}