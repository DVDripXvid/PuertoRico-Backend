using PuertoRico.Engine.DTOs;

namespace PuertoRico.Engine.Events
{
    public class GameChangedEvent : IGameEvent
    {
        public GameDto Game { get; set; }
    }
}