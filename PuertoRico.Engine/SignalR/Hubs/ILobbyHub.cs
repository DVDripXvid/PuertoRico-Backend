using System.Threading.Tasks;
using PuertoRico.Engine.SignalR.Commands;

namespace PuertoRico.Engine.SignalR.Hubs
{
    public interface ILobbyHub
    {
        Task CreateGame(CreateGameCmd cmd);
        Task JoinGame(GenericGameCmd cmd);
        Task LeaveGame(GenericGameCmd cmd);
        Task StartGame(GenericGameCmd cmd);
    }
}