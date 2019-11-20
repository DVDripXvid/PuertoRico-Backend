using PuertoRico.Engine.DTOs;

namespace PuertoRico.Engine.Events
{
    public class GameStartedEvent : IGameEvent
    {
        public string GameId { get; set; }
    }
}