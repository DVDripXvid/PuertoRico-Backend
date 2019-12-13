using System.Collections.Generic;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using PuertoRico.Engine.Domain.Tiles.Plantations;

namespace PuertoRico.Engine.DAL
{
    public class GameEntity : CosmosEntity
    {
        [JsonProperty]
        public string Name { get; set; }
        
        [JsonProperty]
        public ICollection<string> Users { get; set; }

        [JsonProperty]
        public bool IsStarted { get; set; }
        
        [JsonProperty]
        public ICollection<IPlantation> Plantations { get; set; }
        
        [JsonProperty] 
        public int RandomSeed { get; set; }

        public override PartitionKey GetPartitionKey() {
            return new PartitionKey(IsStarted);
        }
    }
}