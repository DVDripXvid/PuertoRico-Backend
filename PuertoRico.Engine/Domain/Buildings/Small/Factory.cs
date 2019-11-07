namespace PuertoRico.Engine.Domain.Buildings.Small
{
    public class Factory : SmallBuilding
    {
        public override int Cost => 7;
        public override int VictoryPoint => 3;
        public override int MaxDiscountByQuarry => 3;
    }
}