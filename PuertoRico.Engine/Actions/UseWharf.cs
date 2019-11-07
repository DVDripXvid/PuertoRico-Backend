using PuertoRico.Engine.Domain.Resources.Goods;

namespace PuertoRico.Engine.Actions
{
    public class UseWharf : IAction
    {
        public ActionType ActionType => ActionType.UseWharf;

        public GoodType GoodType { get; set; }
    }
}