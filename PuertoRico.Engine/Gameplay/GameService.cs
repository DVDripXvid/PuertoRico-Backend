using System.Collections.Generic;
using System.Threading.Tasks;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain;
using PuertoRico.Engine.Exceptions;
using PuertoRico.Engine.Extensions;

namespace PuertoRico.Engine.Gameplay
{
    public class GameService : IGameService
    {

        public Task ExecuteRoleAction(Game game, string userId, IAction action) {
            var player = game.Players.WithUserId(userId);
            if (game.CurrentRoleOwnerPlayer.Role == null) {
                throw new GameException("No role selected for this turn");
            }

            if (game.CurrentRoleOwnerPlayer.Role.CurrentPlayer != player) {
                throw new GameException("Not current player for this role");
            }

            game.CurrentRoleOwnerPlayer.Role.Execute(action, player);
            return Task.CompletedTask;
        }

        public Task ExecuteSelectRole(Game game, string userId, SelectRole selectRole) {
            var player = game.Players.WithUserId(userId);
            if (!game.GetAvailableActionTypes(player).Contains(ActionType.SelectRole)) {
                throw new GameException("SelectRole not available");
            }

            var role = game.Roles[selectRole.RoleIndex];
            player.SelectRole(role, game);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<ActionType>> GetAvailableActionTypeForUser(Game game, string userId) {
            var player = game.Players.WithUserId(userId);
            return Task.FromResult<IEnumerable<ActionType>>(game.GetAvailableActionTypes(player));
        }
    }
}