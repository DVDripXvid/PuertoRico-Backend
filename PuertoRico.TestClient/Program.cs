using System;
using System.Collections;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain;
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
            await gameHub.StartAsync();
            var asd = await gameHub.InvokeAsync<Game>("asd"); 
            await gameHub.InvokeAsync("ExecuteAction", new {
                GameId = "asd",
                Action = new {
                    ActionType = "Build",
                    BuildingIndex = 5,
                }
            });
        }
    }
}