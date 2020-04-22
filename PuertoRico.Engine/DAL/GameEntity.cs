using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using PuertoRico.Engine.Domain;
using PuertoRico.Engine.Domain.Player;
using PuertoRico.Engine.Domain.Tiles.Plantations;

namespace PuertoRico.Engine.DAL
{
    public class GameEntity : CosmosEntity
    {
        [JsonProperty] public string Name { get; set; }

        [JsonProperty] public ICollection<OwnedPlayerEntity> Players { get; set; }

        [JsonProperty] public GameStatus Status { get; set; }

        [JsonProperty] public int RandomSeed { get; set; }

        [JsonProperty] public string Endpoint { get; set; }
        
        [JsonProperty(PropertyName = "ttl", NullValueHandling = NullValueHandling.Ignore)]
        public int? ttl { get; set; }

        public override PartitionKey GetPartitionKey() {
            return new PartitionKey(Id);
        }

        public Game ToModel() {
            var game = new Game(Id, Name, RandomSeed) {Endpoint = Endpoint};
            Players.ToList().ForEach(p => game.Join(new Player(p.UserId, p.Username, p.PictureUrl)));
            return game;
        }

        public static GameEntity Create(Game game) {
            return new GameEntity {
                Id = game.Id,
                Name = game.Name,
                RandomSeed = game.RandomSeed,
                Players = game.Players.Select(p => new OwnedPlayerEntity {
                    Username = p.Username,
                    UserId = p.UserId,
                    PictureUrl = p.PictureUrl,
                }).ToList(),
                Status = game.Status,
                Endpoint = game.Endpoint
            };
        }
    }

    public class OwnedPlayerEntity
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string PictureUrl { get; set; }

        public int? Result { get; set; }
    }
}