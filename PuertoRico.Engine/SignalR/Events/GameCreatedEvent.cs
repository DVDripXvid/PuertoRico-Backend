using PuertoRico.Engine.DTOs;

namespace PuertoRico.Engine.SignalR.Events
{
    public class GameCreatedEvent : IGameEvent
    {
        public string GameId { get; set; }
        public string GameName { get; set; }
        public PlayerDto CreatedBy { get; set; }
    }
}