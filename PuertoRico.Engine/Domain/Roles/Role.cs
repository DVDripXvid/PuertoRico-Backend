﻿using System;
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
        private readonly Dictionary<string, string> _playerPhases = new Dictionary<string, string>();
        protected const string EndedPhase = "ended";
        protected Game Game { get; }
        
        protected Role(Game game) {
            Game = game;
        }

        public void Execute(IAction action, IPlayer player) {
            var availableActionTypes = GetAvailableActionTypes(player);
            if (!availableActionTypes.Contains(action.ActionType)) {
                throw new GameException($"{action.ActionType} is not available for role {Name}");
            }

            ExecuteInternal(action, player, _playerPhases[player.UserId]);
        }

        public virtual void OnSelect(IPlayer roleOwner, IEnumerable<IPlayer> players) {
            roleOwner.Role = this;
            //TODO: add doubloon to role owner
            foreach (var player in players) {
                _playerPhases[player.UserId] = GetInitialPhase(player);
            }
        }

        public virtual void CleanUp() {
            _playerPhases.Clear();
        }

        public HashSet<ActionType> GetAvailableActionTypes(IPlayer player) {
            var playerPhase = _playerPhases[player.UserId];
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

        protected bool HasPrivilege(IPlayer player) {
            return player.Role == this;
        }

        protected void SetPlayerPhase(IPlayer player, string phase) {
            _playerPhases[player.UserId] = phase;
        }

        protected void HandleUnknownPhase(string phase) {
            throw new InvalidOperationException($"{Name} has no {phase} phase");
        }

        protected void HandleUnsupportedAction(IAction action) {
            throw new InvalidOperationException($"{action.ActionType} cannot be executed with {Name} role");
        }

        protected abstract void ExecuteInternal(IAction action, IPlayer player, string getPlayerPhase);

        protected abstract HashSet<ActionType> GetAvailableActionTypesInternal(IPlayer player, string phase);

        protected abstract string GetInitialPhase(IPlayer player);
    }
}