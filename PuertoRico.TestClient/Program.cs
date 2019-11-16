using System;
using System.Collections;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain;
using PuertoRico.Engine.Events;
using PuertoRico.Engine.SignalR;

namespace PuertoRico.TestClient
{
    class Program
    {
        static void Main(string[] args) {
            SampleGame().Wait();
        }

        private static async Task SampleGame() {
            var gameHub = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/game")
                .Build();

            gameHub.On<GameCreatedEvent>("gameCreated", LogJson);
            await gameHub.StartAsync();
            await gameHub.SendAsync("createGame", "testGame");
        }

        private static void LogJson(object any)
        {
            var json = JsonSerializer.Serialize(any);
            Console.WriteLine(json);
        }
    }
}