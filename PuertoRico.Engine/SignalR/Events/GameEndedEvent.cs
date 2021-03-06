﻿using System.Collections.Generic;
using PuertoRico.Engine.DTOs;

namespace PuertoRico.Engine.SignalR.Events
{
    public class GameEndedEvent : IGameEvent
    {
        public string GameId { get; set; }
        public IEnumerable<PlayerResultDto> Results { get; set; }
    }
}