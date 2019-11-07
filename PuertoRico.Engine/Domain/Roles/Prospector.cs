using System;
using System.Collections.Generic;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain.Player;

namespace PuertoRico.Engine.Domain.Roles
{
    public class Prospector : Role
    {
        public Prospector(Game game) : base(game) { }

        public override void OnSelect(IPlayer roleOwner, IEnumerable<IPlayer> players) {
            base.OnSelect(roleOwner, players);
            roleOwner.Doubloons += 1;
        }

        protected override void ExecuteInternal(IAction action, IPlayer player, string getPlayerPhase) {
            throw new InvalidOperationException("Prospector cannot take any actions");
        }

        protected override HashSet<ActionType> GetAvailableActionTypesInternal(IPlayer player, string phase) {
            return new HashSet<ActionType>();
        }

        protected override string GetInitialPhase(IPlayer player) {
            return EndedPhase;
        }
    }
}