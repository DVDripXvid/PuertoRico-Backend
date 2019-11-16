using PuertoRico.Engine.DTOs;

namespace PuertoRico.Engine.Events
{
    public class PlayerJoinedEvent
    {
        public string GameId { get; set; }
        public PlayerDto Player { get; set; }
    }
}