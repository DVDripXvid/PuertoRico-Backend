using PuertoRico.Engine.Domain.Resources.Goods;

namespace PuertoRico.Engine.Actions
{
    public class DeliverGoods : IAction
    {
        public ActionType ActionType => ActionType.DeliverGoods;

        public GoodType GoodType { get; set; }
        public int ShipCapacity { get; set; }
    }
}