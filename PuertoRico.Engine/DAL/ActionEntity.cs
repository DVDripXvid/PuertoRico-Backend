using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using PuertoRico.Engine.Actions;

namespace PuertoRico.Engine.DAL
{
    public class ActionEntity : CosmosEntity
    {
        [JsonProperty]
        public string GameId { get; set; }
        
        [JsonProperty]
        public string UserId { get; set; }

        [JsonProperty]
        public IAction Action { get; set; }

        public override PartitionKey GetPartitionKey() {
            return new PartitionKey(GameId);
        }
    }
}