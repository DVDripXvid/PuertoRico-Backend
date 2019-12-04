using System.Threading.Tasks;
using PuertoRico.Engine.SignalR.Events;

namespace PuertoRico.Engine.SignalR
{
    public interface IGameClient
    {
        Task GameCreated(GameCreatedEvent ev);

        Task GameChanged(GameChangedEvent ev);

        Task GameEnded(GameEndedEvent ev);

        Task GameStarted(GameStartedEvent ev);

        Task PlayerJoined(PlayerJoinedEvent ev);

        Task PlayerLeft(PlayerLeftEvent ev);
    }
}