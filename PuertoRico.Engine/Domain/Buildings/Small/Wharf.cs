﻿namespace PuertoRico.Engine.Domain.Buildings.Small
{
    public class Wharf : SmallBuilding
    {
        public override int Cost => 9;
        public override int VictoryPoint => 3;
        public override int MaxDiscountByQuarry => 3;
    }
}