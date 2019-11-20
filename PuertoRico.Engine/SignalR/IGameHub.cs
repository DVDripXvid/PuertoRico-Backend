using System.Threading.Tasks;

namespace PuertoRico.Engine.SignalR
{
    public interface IGameHub
    {
        Task CreateGame(string name);
        Task JoinGame(string gameId);
        Task LeaveGame(string gameId);
        Task StartGame(string gameId);
    }
}