using PuertoRico.Engine.DTOs;

namespace PuertoRico.Engine.Events
{
    public class PlayerLeftEvent
    {
        public string GameId { get; set; }
        public PlayerDto Player { get; set; }
    }
}