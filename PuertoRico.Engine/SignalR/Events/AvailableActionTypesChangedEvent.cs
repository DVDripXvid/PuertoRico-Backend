using System.Collections.Generic;
using PuertoRico.Engine.Actions;

namespace PuertoRico.Engine.SignalR.Events
{
    public class AvailableActionTypesChangedEvent
    {
        public string GameId { get; set; }
        public IEnumerable<ActionType> ActionTypes { get; set; }
    }
}