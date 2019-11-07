using PuertoRico.Engine.Domain.Resources.Goods;

namespace PuertoRico.Engine.Actions
{
    public class SellGood : IAction
    {
        public ActionType ActionType => ActionType.SellGood;
        public GoodType GoodType { get; set; }
    }
}