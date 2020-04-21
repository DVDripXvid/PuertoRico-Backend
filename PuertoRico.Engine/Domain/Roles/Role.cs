using System;
using System.Collections.Generic;
using System.Linq;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain.Player;
using PuertoRico.Engine.Exceptions;

namespace PuertoRico.Engine.Domain.Roles
{
    public abstract class Role : IRole
    {
        public virtual string Name => GetType().Name;
        public IPlayer CurrentPlayer { get; private set; }
        private readonly Dictionary<string, string> _playerPhases = new Dictionary<string, string>();
        protected const string EndedPhase = "ended";
        public int StackedDoubloons { get; private set; }
        protected Game Game { get; }

        protected Role(Game game) {
            Game = game;
        }

        public void AddOneDoubloon() {
            StackedDoubloons += 1;
        }

        public void Execute(IAction action, IPlayer player) {
            if (action.ActionType == ActionType.EndRole) {
                MoveToNextPlayer();
                return;
            }

            var availableActionTypes = GetAvailableActionTypes(player);
            if (!availableActionTypes.Contains(action.ActionType)) {
                throw new GameException($"{action.ActionType} is not available for role {Name}");
            }

            ExecuteInternal(action, player, _playerPhases[player.UserId]);
        }

        public virtual void OnSelect(IPlayer roleOwner) {
            CurrentPlayer = roleOwner;
            roleOwner.Doubloons += StackedDoubloons;
            StackedDoubloons = 0;
            foreach (var player in Game.Players) {
                _playerPhases[player.UserId] = GetInitialPhase(player);
            }
        }

        public virtual void CleanUp() {
            _playerPhases.Clear();
        }

        public HashSet<ActionType> GetAvailableActionTypes(IPlayer player) {
            if (_playerPhases.TryGetValue(player.UserId, out var playerPhase)) {
                if (playerPhase == EndedPhase) {
                    return new HashSet<ActionType> {
                        ActionType.EndRole
                    };
                }

                var availableActionTypes = GetAvailableActionTypesInternal(player, playerPhase);
                if (availableActionTypes.Count == 0) {
                    throw new InvalidOperationException($"no available actions");
                }

                return availableActionTypes;   
            }
            return new HashSet<ActionType>();
        }

        protected bool HasPrivilege(IPlayer player) {
            return player.Role == this;
        }

        protected void SetPlayerPhase(IPlayer player, string phase) {
            _playerPhases[player.UserId] = phase;
        }

        protected void HandleUnknownPhase(string phase) {
            throw new InvalidOperationException($"{Name} has no {phase} phase");
        }

        protected void MoveToNextPlayer() {
            if (_playerPhases.Values.All(p => p == EndedPhase)) {
                Game.MoveToNextPlayer();
            }
            else {
                CurrentPlayer = Game.GetNextPlayerTo(CurrentPlayer);
            }
        }

        protected void HandleUnsupportedAction(IAction action) {
            throw new InvalidOperationException($"{action.ActionType} cannot be executed with {Name} role");
        }

        protected abstract void ExecuteInternal(IAction action, IPlayer player, string getPlayerPhase);

        protected abstract HashSet<ActionType> GetAvailableActionTypesInternal(IPlayer player, string phase);

        protected abstract string GetInitialPhase(IPlayer player);
    }
}