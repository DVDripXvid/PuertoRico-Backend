using PuertoRico.Engine.Domain.Resources.Goods;

namespace PuertoRico.Engine.Actions
{
    public class StoreGoods : IAction
    {
        public ActionType ActionType => ActionType.StoreGoods;

        public GoodType? DefaultStorage { get; set; }
        public GoodType? SmallWarehouse { get; set; }
        public GoodType? LargeWarehouse1 { get; set; }
        public GoodType? LargeWarehouse2 { get; set; }
    }
}