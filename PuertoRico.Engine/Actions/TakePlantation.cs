namespace PuertoRico.Engine.Actions
{
    public class TakePlantation : IAction
    {
        public ActionType ActionType => ActionType.TakePlantation;

        public int TileIndex { get; set; }
    }
}