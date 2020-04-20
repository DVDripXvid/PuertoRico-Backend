using PuertoRico.Engine.DTOs;

namespace PuertoRico.Engine.SignalR.Events
{
    public class GameStartedEvent : IGameEvent
    {
        public GameSummaryDto Game { get; set; }
    }
}