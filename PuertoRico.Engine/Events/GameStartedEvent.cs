using PuertoRico.Engine.DTOs;

namespace PuertoRico.Engine.Events
{
    public class GameStartedEvent
    {
        public PlayerDto StartedBy { get; set; }
        public string GameId { get; set; }
    }
}