using System;
using System.Collections.Generic;
using System.Linq;
using PuertoRico.Engine.Domain.Resources.Goods;

namespace PuertoRico.Engine.Domain.Misc
{
    public class CargoShip
    {
        public GoodType? GoodType { get; private set; }
        public int GoodCount => _goods.Count;
        public int Capacity { get; }
        private readonly List<IGood> _goods;

        public CargoShip(int capacity) {
            Capacity = capacity;
            _goods = new List<IGood>(capacity);
        }

        public bool IsFull() {
            return _goods.Count == Capacity;
        }

        public bool IsNotFull() {
            return _goods.Count < Capacity;
        }

        public bool IsEmpty() {
            return _goods.Count == 0;
        }

        public bool HasGoodType(GoodType goodType) {
            return !IsEmpty() && GoodType == goodType;
        }

        public bool CanBeLoaded(GoodType goodType) {
            return IsEmpty() || (!IsFull() && HasGoodType(goodType));
        }

        public int Load(List<IGood> goodsToLoad) {
            if (goodsToLoad.Count > 0 && CanBeLoaded(goodsToLoad.First().Type)) {
                var barrelCount = Math.Min(Capacity - _goods.Count, goodsToLoad.Count);
                _goods.AddRange(goodsToLoad.Take(barrelCount));
                GoodType = goodsToLoad.First().Type;
                return barrelCount;
            }

            return 0;
        }

        public List<IGood> ReleaseLoadedGoods() {
            var releasedGoods = new List<IGood>(_goods);
            _goods.Clear();
            return releasedGoods;
        }
    }
}