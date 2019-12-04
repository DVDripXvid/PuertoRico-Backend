using PuertoRico.Engine.DTOs;

namespace PuertoRico.Engine.SignalR.Events
{
    public class GameChangedEvent : IGameEvent
    {
        public GameDto Game { get; set; }
    }
}