namespace PuertoRico.Engine.SignalR.Events
{
    public class GameStartedEvent : IGameEvent
    {
        public string GameId { get; set; }
    }
}