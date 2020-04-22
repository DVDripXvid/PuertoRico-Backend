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
        private const string SelectTileOrHaciendaPhase = "select_tile_or_hacienda";

        public Settler(Game game) : base(game) { }

        public override void CleanUp() {
            base.CleanUp();
            Game.PlantationDeck.DiscardAndDrawNew();
        }

        protected override HashSet<ActionType> GetAvailableActionTypesInternal(IPlayer player, string phase) {
            var actions = new HashSet<ActionType> {ActionType.EndPhase};
            switch (phase) {
                case SelectTileOrHaciendaPhase:
                    if (CanTakePlantation(player))
                        actions.Add(ActionType.TakePlantation);

                    if (CanTakeQuarry(player))
                        actions.Add(ActionType.TakeQuarry);

                    if (CanUseHacienda(player))
                        actions.Add(ActionType.TakeRandomPlantation);

                    break;

                case SelectTilePhase:
                    if (CanTakePlantation(player))
                        actions.Add(ActionType.TakePlantation);

                    if (CanTakeQuarry(player))
                        actions.Add(ActionType.TakeQuarry);

                    break;


                case HaciendaPhase:
                    if (CanUseHacienda(player))
                        actions.Add(ActionType.TakeRandomPlantation);

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
                    ExecuteTakePlantation(takePlantation, player, phase);
                    break;
                case TakeQuarry _:
                    ExecuteTakeQuarry(player, phase);
                    break;
                case TakeRandomPlantation _:
                    ExecuteTakeRandomPlantation(player, phase);
                    break;
                case EndPhase _:
                    SetPlayerPhase(player, EndedPhase);
                    break;
                default:
                    HandleUnsupportedAction(action);
                    break;
            }
        }

        protected override string GetInitialPhase(IPlayer player) {
            return CanUseHacienda(player) ? SelectTileOrHaciendaPhase : SelectTilePhase;
        }

        private void ExecuteTakePlantation(TakePlantation takePlantation, IPlayer player, string phase) {
            var plantation = Game.PlantationDeck.DrawOneVisible(takePlantation.TileIndex);
            if (player.Buildings.ContainsWorkingOfType<Hospice>() && Game.Colonists.Count > 0) {
                var colonist = Game.Colonists.Pop();
                plantation.AddWorker(colonist);
            }

            player.Tiles.Add(plantation);
            
            if (phase == SelectTileOrHaciendaPhase && CanUseHacienda(player)) {
                SetPlayerPhase(player, HaciendaPhase);
            }
            else {
                SetPlayerPhase(player, EndedPhase);
            }
        }

        private void ExecuteTakeQuarry(IPlayer player, string phase) {
            var quarry = Game.Quarries.Pop();
            if (player.Buildings.ContainsWorkingOfType<Hospice>() && Game.Colonists.Count > 0) {
                var colonist = Game.Colonists.Pop();
                quarry.AddWorker(colonist);
            }

            player.Tiles.Add(quarry);
            if (phase == SelectTileOrHaciendaPhase && CanUseHacienda(player)) {
                SetPlayerPhase(player, HaciendaPhase);
            }
            else {
                SetPlayerPhase(player, EndedPhase);
            }
        }

        private void ExecuteTakeRandomPlantation(IPlayer player, string phase) {
            var plantation = Game.PlantationDeck.DrawRandom();
            player.Tiles.Add(plantation);
            
            if (phase == SelectTileOrHaciendaPhase && CanTakePlantation(player)) {
                SetPlayerPhase(player, SelectTilePhase);
            }
            else {
                SetPlayerPhase(player, EndedPhase);
            }
        }

        private bool CanUseHacienda(IPlayer player) {
            return player.Buildings.ContainsWorkingOfType<Hacienda>() && !player.Tiles.IsFull();
        }

        private bool CanTakePlantation(IPlayer player) {
            return !player.Tiles.IsFull();
        }

        private bool CanTakeQuarry(IPlayer player) {
            return (HasPrivilege(player) || player.Buildings.ContainsWorkingOfType<ConstructionHut>())
                   && Game.Quarries.Count > 0 && !player.Tiles.IsFull();
        }
    }
}