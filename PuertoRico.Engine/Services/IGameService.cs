using System.Collections.Generic;
using System.Threading.Tasks;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain;
using PuertoRico.Engine.Domain.Player;

namespace PuertoRico.Engine.Services
{
    public interface IGameService
    {
        Task ExecuteRoleAction(Game game, string userId, IAction action);
        Task ExecuteSelectRole(Game game, string userId, SelectRole role);
        Task<IEnumerable<ActionType>> GetAvailableActionTypeForUser(Game game, string userId);
    }
}