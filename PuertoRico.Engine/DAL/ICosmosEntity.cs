using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace PuertoRico.Engine.DAL
{
    public abstract class CosmosEntity
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public abstract PartitionKey GetPartitionKey();
    }
}