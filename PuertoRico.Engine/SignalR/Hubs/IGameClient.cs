using System.Threading.Tasks;
using PuertoRico.Engine.SignalR.Events;

namespace PuertoRico.Engine.SignalR.Hubs
{
    public interface IGameClient
    {
        Task GameChanged(GameChangedEvent ev);

        Task GameEnded(GameEndedEvent ev);

        Task AvailableActionTypesChanged(AvailableActionTypesChangedEvent ev);
        
        Task Error(GameErrorEvent ev);
    }
}