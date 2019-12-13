using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain;
using PuertoRico.Engine.Domain.Player;
using PuertoRico.Engine.Exceptions;
using PuertoRico.Engine.Extensions;

namespace PuertoRico.Engine.Services
{
    public class GameService : IGameService
    {
        public Task ExecuteRoleAction(Game game, string userId, IAction action) {
            var player = game.Players.WithUserId(userId);
            lock (game) {
                if (game.CurrentRoleOwnerPlayer.Role == null) {
                    throw new GameException("No role selected for this turn");
                }

                if (game.CurrentRoleOwnerPlayer.Role.CurrentPlayer != player) {
                    throw new GameException("Not the current player for this role");
                }

                game.CurrentRoleOwnerPlayer.Role.Execute(action, player);

                EndRoleForPlayerIfNeeded(game, player);
            }

            return Task.CompletedTask;
        }

        public Task ExecuteSelectRole(Game game, string userId, SelectRole selectRole) {
            var player = game.Players.WithUserId(userId);
            lock (game) {
                if (!game.GetAvailableActionTypes(player).Contains(ActionType.SelectRole)) {
                    throw new GameException("SelectRole not available");
                }

                var role = game.Roles[selectRole.RoleIndex];
                player.SelectRole(role, game);

                EndRoleForPlayerIfNeeded(game, player);
            }

            return Task.CompletedTask;
        }

        public Task<IEnumerable<ActionType>> GetAvailableActionTypeForUser(Game game, string userId) {
            lock (game) {
                var player = game.Players.WithUserId(userId);
                var currentPlayer = game.GetCurrentPlayer();
                var availableActions = currentPlayer == player
                    ? game.GetAvailableActionTypes(player)
                    : new HashSet<ActionType>();
                return Task.FromResult<IEnumerable<ActionType>>(availableActions);
            }
        }

        private void EndRoleForPlayerIfNeeded(Game game, IPlayer player) {
            var availableActionTypes = game.GetAvailableActionTypes(player);
            if (availableActionTypes.Count == 1 && availableActionTypes.Single() == ActionType.EndRole) {
                game.CurrentRoleOwnerPlayer.Role.Execute(new EndRole(), player);
            }
        }
    }
}