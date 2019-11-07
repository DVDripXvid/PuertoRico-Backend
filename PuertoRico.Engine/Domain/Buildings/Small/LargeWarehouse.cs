using System.Collections.Generic;
using System.Linq;
using PuertoRico.Engine.Domain.Player;
using PuertoRico.Engine.Domain.Resources.Goods;

namespace PuertoRico.Engine.Domain.Buildings.Small
{
    public class LargeWarehouse : SmallBuilding
    {
        public override int Cost => 6;
        public override int VictoryPoint => 2;
        public override int MaxDiscountByQuarry => 2;
        private readonly List<IGood> _store1 = new List<IGood>();
        private readonly List<IGood> _store2 = new List<IGood>();

        public void StoreGoods(GoodType goodType, IPlayer player) {
            var goodsToStore = player.Goods.Where(g => g.Type == goodType).ToList();
            if (_store1.Count == 0) {
                _store1.AddRange(goodsToStore);
            }
            else if(_store2.Count == 0) {
                _store2.AddRange(goodsToStore);
            }
            
            player.Goods.RemoveAll(g => g.Type == goodType);
        }

        public List<IGood> ReleaseGoods() {
            var goods = new List<IGood>(_store1.Count + _store2.Count);
            goods.AddRange(_store1);
            goods.AddRange(_store2);
            _store1.Clear();
            _store2.Clear();
            return goods;
        }
    }
}