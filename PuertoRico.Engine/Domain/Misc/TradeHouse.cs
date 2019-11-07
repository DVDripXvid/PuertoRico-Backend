using System.Collections.Generic;
using PuertoRico.Engine.Domain.Buildings.Small;
using PuertoRico.Engine.Domain.Player;
using PuertoRico.Engine.Domain.Resources.Goods;
using PuertoRico.Engine.Exceptions;

namespace PuertoRico.Engine.Domain.Misc
{
    public class TradeHouse
    {
        public readonly List<IGood> Goods = new List<IGood>(4);

        private static readonly Dictionary<GoodType, int> PriceList = new Dictionary<GoodType, int> {
            {GoodType.Corn, 0},
            {GoodType.Indigo, 1},
            {GoodType.Sugar, 2},
            {GoodType.Tobacco, 3},
            {GoodType.Coffee, 4},
        };

        public bool IsFull => Goods.Count == 4;

        public bool CanBeSoldBy(GoodType goodType, IPlayer player) {
            return !IsFull
                   && player.Goods.Exists(g => g.Type == goodType)
                   && (!Goods.Exists(g => g.Type == goodType)
                       || player.Buildings.ContainsWorkingOfType<Office>());
        }

        public int Sell(IGood good) {
            Goods.Add(good);
            return PriceList[good.Type];
        }
    }
}