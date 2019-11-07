using System.Collections.Generic;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain.Buildings.Small;
using PuertoRico.Engine.Domain.Player;

namespace PuertoRico.Engine.Domain.Roles
{
    public class Settler : Role
    {
        private const string SelectTilePhase = "select_tile";
        private const string HaciendaPhase = "hacienda";

        public Settler(Game game) : base(game) { }

        public override void CleanUp() {
            base.CleanUp();
            Game.PlantationDeck.DiscardAndDrawNew();
        }

        protected override HashSet<ActionType> GetAvailableActionTypesInternal(IPlayer player, string phase) {
            var actions = new HashSet<ActionType> {ActionType.EndPhase};
            switch (phase) {
                case SelectTilePhase:
                    if (HasPrivilege(player) && Game.Quarries.Count > 0) {
                        actions.Add(ActionType.TakeQuarry);
                    }

                    if (!player.Tiles.IsFull()) {
                        actions.Add(ActionType.TakePlantation);
                    }

                    break;
                case HaciendaPhase:
                    if (!player.Tiles.IsFull()) {
                        actions.Add(ActionType.TakeRandomPlantation);
                    }

                    break;
                default:
                    HandleUnknownPhase(phase);
                    break;
            }

            return actions;
        }

        protected override void ExecuteInternal(IAction action, IPlayer player, string phase) {
            switch (action) {
                case TakePlantation takePlantation:
                    ExecuteTakePlantation(takePlantation, player);
                    break;
                case TakeQuarry _:
                    ExecuteTakeQuarry(player);
                    break;
                case TakeRandomPlantation _:
                    ExecuteTakeRandomPlantation(player);
                    break;
                case EndPhase _:
                    SetPlayerPhase(player, phase == HaciendaPhase
                        ? SelectTilePhase
                        : EndedPhase);
                    break;
                default:
                    HandleUnsupportedAction(action);
                    break;
            }
        }

        protected override string GetInitialPhase(IPlayer player) {
            return player.Buildings.ContainsWorkingOfType<Hacienda>()
                ? HaciendaPhase
                : SelectTilePhase;
        }

        private void ExecuteTakePlantation(TakePlantation takePlantation, IPlayer player) {
            var plantation = Game.PlantationDeck.DrawOneVisible(takePlantation.TileIndex);
            if (player.Buildings.ContainsWorkingOfType<Hospice>() && Game.Colonists.Count > 0) {
                var colonist = Game.Colonists.Pop();
                plantation.AddWorker(colonist);
            }

            player.Tiles.Add(plantation);
            SetPlayerPhase(player, EndedPhase);
        }

        private void ExecuteTakeQuarry(IPlayer player) {
            var quarry = Game.Quarries.Pop();
            if (player.Buildings.ContainsWorkingOfType<Hospice>() && Game.Colonists.Count > 0) {
                var colonist = Game.Colonists.Pop();
                quarry.AddWorker(colonist);
            }

            player.Tiles.Add(quarry);
            SetPlayerPhase(player, EndedPhase);
        }

        private void ExecuteTakeRandomPlantation(IPlayer player) {
            var plantation = Game.PlantationDeck.DrawRandom();
            player.Tiles.Add(plantation);
            SetPlayerPhase(player, SelectTilePhase);
        }
    }
}