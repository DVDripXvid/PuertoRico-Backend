namespace PuertoRico.Engine.Actions
{
    public class PlaceColonist : IAction
    {
        public ActionType ActionType => ActionType.PlaceColonist;
        public int ToIndex { get; set; }
        public bool IsPlaceToTile { get; set; }
    }
}