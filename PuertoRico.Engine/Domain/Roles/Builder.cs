using System;
using System.Collections.Generic;
using System.Linq;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain.Buildings.Small;
using PuertoRico.Engine.Domain.Player;
using PuertoRico.Engine.Exceptions;

namespace PuertoRico.Engine.Domain.Roles
{
    public class Builder : Role
    {
        private const string BuildPhase = "build";

        public Builder(Game game) : base(game) { }

        protected override void ExecuteInternal(IAction action, IPlayer player, string playerPhase) {
            switch (action) {
                case Build build:
                    ExecuteBuild(build, player);
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
            if (phase != BuildPhase) {
                HandleUnknownPhase(phase);
            }

            var actions = new HashSet<ActionType> {ActionType.EndPhase};
            if (!player.Buildings.IsFull()) {
                actions.Add(ActionType.Build);
            }

            return actions;
        }

        protected override string GetInitialPhase(IPlayer player) {
            return BuildPhase;
        }

        private void ExecuteBuild(Build build, IPlayer player) {
            var building = Game.Buildings[build.BuildingIndex];
            var quarryCount = player.Tiles.Quarries.Count();
            var price = building.Cost - Math.Min(quarryCount, building.MaxDiscountByQuarry);
            if (HasPrivilege(player)) {
                price = Math.Max(0, price - 1);
            }
            if (player.Doubloons < price) {
                throw new GameException($"Insufficient funds");
            }

            player.Doubloons -= price;
            Game.Buildings.Remove(building);
            player.Build(building);

            if (player.Buildings.ContainsWorkingOfType<University>()) {
                if (Game.Colonists.Count > 0) {
                    building.AddWorker(Game.Colonists.Pop());
                } else if (!Game.ColonistsShip.IsEmpty()) {
                    building.AddWorker(Game.ColonistsShip.TakeColonist());
                }
            }
            
            SetPlayerPhase(player, EndedPhase);
        }
    }
}