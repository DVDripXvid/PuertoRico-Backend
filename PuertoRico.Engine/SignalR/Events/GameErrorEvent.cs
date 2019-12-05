namespace PuertoRico.Engine.SignalR.Events
{
    public class GameErrorEvent : IGameEvent
    {
        public string ErrorMessage { get; set; }
    }
}