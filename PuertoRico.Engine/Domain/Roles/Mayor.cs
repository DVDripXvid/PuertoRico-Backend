using System.Collections.Generic;
using System.Linq;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain.Player;

namespace PuertoRico.Engine.Domain.Roles
{
    public class Mayor : Role
    {
        private const string ColonisationPhase = "colonise";

        public Mayor(Game game) : base(game) { }

        public override void OnSelect(IPlayer roleOwner) {
            base.OnSelect(roleOwner);
            if (Game.Colonists.Count > 0) {
                roleOwner.AddColonist(Game.Colonists.Pop());
            }

            var ship = Game.ColonistsShip;
            var currentPlayer = roleOwner;
            while (!ship.IsEmpty()) {
                var colonist = ship.TakeColonist();
                currentPlayer.AddColonist(colonist);
                currentPlayer = Game.GetNextPlayerTo(currentPlayer);
            }
        }

        public override void CleanUp() {
            base.CleanUp();
            Game.ColonistsShip.RecalculateCapacityAndRefill(Game);
        }

        protected override void ExecuteInternal(IAction action, IPlayer player, string playerPhase) {
            switch (action) {
                case MoveColonist moveColonist:
                    ExecuteMoveColonist(moveColonist, player);
                    break;
                case PlaceColonist placeColonist:
                    ExecutePlaceColonist(placeColonist, player);
                    break;
                case EndPhase _:
                    SetPlayerPhase(player, EndedPhase);
                    break;
                default:
                    HandleUnsupportedAction(action);
                    break;
            }
        }

        protected override HashSet<ActionType> GetAvailableActionTypesInternal(IPlayer player, string phase) {
            if (phase != ColonisationPhase) {
                HandleUnknownPhase(phase);
            }

            var actions = new HashSet<ActionType> {ActionType.MoveColonist, ActionType.EndPhase};
            if (player.IdleColonists.Any()) {
                actions.Add(ActionType.PlaceColonist);
            }

            return actions;
        }

        protected override string GetInitialPhase(IPlayer player) {
            return ColonisationPhase;
        }

        private void ExecuteMoveColonist(MoveColonist moveColonist, IPlayer player) {
            var colonist = moveColonist.IsMoveFromTile
                ? player.Tiles[moveColonist.FromIndex].RemoveWorker()
                : player.Buildings[moveColonist.FromIndex].RemoveWorker();
            if (moveColonist.IsMoveToTile) {
                player.Tiles[moveColonist.ToIndex].AddWorker(colonist);
            }
            else {
                player.Buildings[moveColonist.ToIndex].AddWorker(colonist);
            }
        }

        private void ExecutePlaceColonist(PlaceColonist placeColonist, IPlayer player) {
            var colonist = player.IdleColonists.First();
            player.IdleColonists.Remove(colonist);

            if (placeColonist.IsPlaceToTile) {
                player.Tiles[placeColonist.ToIndex].AddWorker(colonist);
            }
            else {
                player.Buildings[placeColonist.ToIndex].AddWorker(colonist);
            }
        }
    }
}