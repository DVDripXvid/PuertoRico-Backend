using System.Threading.Tasks;
using PuertoRico.Engine.SignalR.Events;

namespace PuertoRico.Engine.SignalR.Hubs
{
    public interface ILobbyClient
    {
        Task GameCreated(GameCreatedEvent ev);

        Task GameDestroyed(GameDestroyedEvent ev);
        
        Task GameStarted(GameStartedEvent ev);

        Task PlayerJoined(PlayerJoinedEvent ev);

        Task PlayerLeft(PlayerLeftEvent ev);
        
        Task GameEnded(GameEndedEvent ev);
    }
}