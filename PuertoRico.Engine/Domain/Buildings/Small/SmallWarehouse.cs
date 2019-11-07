using System.Collections.Generic;
using System.Linq;
using PuertoRico.Engine.Domain.Player;
using PuertoRico.Engine.Domain.Resources.Goods;

namespace PuertoRico.Engine.Domain.Buildings.Small
{
    public class SmallWarehouse : SmallBuilding
    {
        public override int Cost => 3;
        public override int VictoryPoint => 1;
        public override int MaxDiscountByQuarry => 1;
        private readonly List<IGood> _storedGoods = new List<IGood>();

        public void StoreGoods(GoodType goodType, IPlayer player) {
            var goodsToStore = player.Goods.Where(g => g.Type == goodType).ToList();
            _storedGoods.AddRange(goodsToStore);
            player.Goods.RemoveAll(g => g.Type == goodType);
        }
        
        public List<IGood> ReleaseGoods() {
            var goods = new List<IGood>(_storedGoods);
            _storedGoods.Clear();
            return goods;
        }
    }
}