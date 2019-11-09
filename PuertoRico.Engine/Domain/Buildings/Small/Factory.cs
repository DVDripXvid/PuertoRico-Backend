using System.Collections.Generic;

namespace PuertoRico.Engine.Domain.Buildings.Small
{
    public class Factory : SmallBuilding
    {
        public override int Cost => 7;
        public override int VictoryPoint => 3;
        public override int MaxDiscountByQuarry => 3;

        public static readonly Dictionary<int, int> BonusForProduction = new Dictionary<int, int> {
            {0, 0},
            {1, 0},
            {2, 1},
            {3, 2},
            {4, 3},
            {5, 5},
        };
    }
}