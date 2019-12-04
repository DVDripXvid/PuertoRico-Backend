namespace PuertoRico.Engine.SignalR.Events
{
    public class GameEndedEvent : IGameEvent
    {
        public string GameId { get; set; }
    }
}