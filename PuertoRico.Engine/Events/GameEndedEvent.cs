namespace PuertoRico.Engine.Events
{
    public class GameEndedEvent : IGameEvent
    {
        public string GameId { get; set; }
    }
}