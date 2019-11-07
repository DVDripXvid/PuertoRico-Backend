using PuertoRico.Engine.Domain.Resources.Goods;

namespace PuertoRico.Engine.Actions
{
    public class BonusProduction : IAction
    {
        public ActionType ActionType => ActionType.BonusProduction;
        public GoodType GoodType { get; set; }
    }
}