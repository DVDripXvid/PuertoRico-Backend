using PuertoRico.Engine.DTOs;

namespace PuertoRico.Engine.Events
{
    public class GameChangedEvent
    {
        public GameDto Game { get; set; }
    }
}