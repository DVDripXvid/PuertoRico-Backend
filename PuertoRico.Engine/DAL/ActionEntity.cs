using Newtonsoft.Json;
using PuertoRico.Engine.Actions;

namespace PuertoRico.Engine.DAL
{
    public class ActionEntity
    {
        [JsonProperty]
        public string GameId { get; set; }

        [JsonProperty]
        public string UserId { get; set; }

        [JsonProperty]
        public IAction Action { get; set; }
    }
}