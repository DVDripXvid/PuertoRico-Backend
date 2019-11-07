namespace PuertoRico.Engine.Actions
{
    public class Build : IAction
    {
        public ActionType ActionType => ActionType.Build;

        public int BuildingIndex { get; set; }
    }
}