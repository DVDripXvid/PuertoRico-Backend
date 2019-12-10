namespace PuertoRico.Engine.SignalR.Events
{
    public class GameDestroyedEvent : IGameEvent
    {
        public string GameId { get; set; }
    }
}