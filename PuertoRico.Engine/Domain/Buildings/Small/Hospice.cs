namespace PuertoRico.Engine.Domain.Buildings.Small
{
    public class Hospice : SmallBuilding
    {
        public override int Cost => 4;
        public override int VictoryPoint => 2;
        public override int MaxDiscountByQuarry => 2;
    }
}