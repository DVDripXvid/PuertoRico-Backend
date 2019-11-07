namespace PuertoRico.Engine.Actions
{
    public class MoveColonist : IAction
    {
        public ActionType ActionType => ActionType.MoveColonist;

        public int FromIndex { get; set; }
        public int ToIndex { get; set; }
        public bool IsMoveFromTile { get; set; }
        public bool IsMoveToTile { get; set; }
    }
}