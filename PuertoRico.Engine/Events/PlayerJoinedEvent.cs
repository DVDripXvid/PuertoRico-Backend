using PuertoRico.Engine.DTOs;

namespace PuertoRico.Engine.Events
{
    public class PlayerJoinedEvent : IGameEvent
    {
        public string GameId { get; set; }
        public PlayerDto Player { get; set; }
    }
}