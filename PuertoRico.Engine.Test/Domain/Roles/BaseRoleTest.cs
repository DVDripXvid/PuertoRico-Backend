﻿using System;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain;
using PuertoRico.Engine.Domain.Player;
using PuertoRico.Engine.Domain.Roles;
using Xunit;

namespace PuertoRico.Engine.Test.Domain.Roles
{
    public abstract class BaseRoleTest<T> where T : IRole
    {
        protected readonly IRole Role;
        protected readonly Game Game;
        protected readonly IPlayer RoleOwner;
        protected readonly int DoubloonsOnRole;

        public BaseRoleTest() {
            DoubloonsOnRole = 3;
            Game = new Game(3);
            Role = (T)Activator.CreateInstance(typeof(T), Game);
            for (var i = 0; i < DoubloonsOnRole; i++) {
                Role.AddOneDoubloon();
            }
            RoleOwner = Game.CurrentPlayer;
            RoleOwner.SelectRole(Role);
        }

        [Fact]
        public void StackedDoubloonsMovedToPlayer() {
            Assert.Equal(0, Role.StackedDoubloons);
            Assert.Equal(DoubloonsOnRole, RoleOwner.Doubloons);
        }
        
        protected void CanSkipPhase(IPlayer player, ActionType nextExpectedActionType) {
            var actions = Game.GetAvailableActionTypes(player);
            Assert.Contains(ActionType.EndPhase, actions);
            
            var action = new EndPhase();
            Role.Execute(action, player);

            actions = Game.GetAvailableActionTypes(player);
            Assert.Contains(nextExpectedActionType, actions);
        }

        protected void CanExecuteActionOnce(IAction action, IPlayer player) {
            var actions = Game.GetAvailableActionTypes(player);
            Assert.Contains(action.ActionType, actions);
            
            Role.Execute(action, player);

            actions = Game.GetAvailableActionTypes(player);
            Assert.DoesNotContain(action.ActionType, actions);
        }
        
        protected void CanExecuteActionMultiple(IAction action, IPlayer player) {
            var actions = Game.GetAvailableActionTypes(player);
            Assert.Contains(action.ActionType, actions);
            
            Role.Execute(action, player);

            actions = Game.GetAvailableActionTypes(player);
            Assert.Contains(action.ActionType, actions);
        }
        
        protected IPlayer GetPlayerWithoutPrivilege() {
            return Game.Players.Find(p => p != RoleOwner);
        }

        protected void ReselectRole() {
            RoleOwner.SelectRole(Role);
        }

    }
}