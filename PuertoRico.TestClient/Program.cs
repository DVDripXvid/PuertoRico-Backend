using System;
using System.Collections;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI.Internal;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain;
using PuertoRico.Engine.Domain.Tiles.Plantations;
using PuertoRico.Engine.Events;
using PuertoRico.Engine.SignalR;

namespace PuertoRico.TestClient
{
    class Program
    {
        private static volatile int _playerCount = 0;
        
        static void Main(string[] args) {
            SampleGame().Wait();
            while (true) {
                Thread.Sleep(1500);
                var cmd = Console.ReadLine();
                if (cmd?.Trim().ToLower() == "exit") {
                    break;
                }
            }
        }

        private static async Task SampleGame() {
            var gameHub = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/game")
                .ConfigureLogging(logging => {
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Information);
                })
                .AddJsonProtocol()
                .Build();
            
            gameHub.On<GameCreatedEvent>("gameCreated", ev => {
                LogJson(ev);
                ++_playerCount;
                gameHub.SendAsync("joinGame", ev.GameId);
                gameHub.SendAsync("joinGame", ev.GameId);
            });
            gameHub.On<PlayerJoinedEvent>("playerJoined", ev => {
                LogJson(ev);
                if (++_playerCount >= 3) {
                    gameHub.SendAsync("startGame", ev.GameId);
                }
            });
            gameHub.On<GameStartedEvent>("gameStarted", LogJson);
            gameHub.On<GameChangedEvent>("gameChanged", LogJson);
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