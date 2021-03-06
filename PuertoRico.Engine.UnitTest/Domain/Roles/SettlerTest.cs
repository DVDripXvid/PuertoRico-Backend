﻿using System.Linq;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain.Buildings.Small;
using PuertoRico.Engine.Domain.Player;
using PuertoRico.Engine.Domain.Resources;
using PuertoRico.Engine.Domain.Roles;
using PuertoRico.Engine.Exceptions;
using Xunit;

namespace PuertoRico.Engine.UnitTest.Domain.Roles
{
    public class SettlerTest : BaseRoleTest<Settler>
    {
        [Fact]
        public void CanSkipPlant() {
            CanSkipPhase(GetPlayerWithoutPrivilege(), ActionType.EndRole);
        }

        [Fact]
        public void CanTakePlantation() {
            var action = new TakePlantation {
                TileIndex = 0,
            };
            CanExecuteActionOnce(action, RoleOwner);
        }

        [Fact]
        public void CanTakeQuarryUsingPrivilege() {
            var action = new TakeQuarry();
            CanExecuteActionOnce(action, RoleOwner);
        }

        [Fact]
        public void CanTakeQuarryUsingConstructionHut() {
            var player = GetPlayerWithoutPrivilege();
            var constructionHut = new ConstructionHut();
            constructionHut.AddWorker(new Colonist());
            player.Build(constructionHut);
            var action = new TakeQuarry();
            CanExecuteActionOnce(action, player);
        }

        [Fact]
        public void CanUseHacienda() {
            AddHaciendaTo(RoleOwner);
            var action = new TakeRandomPlantation();
            CanExecuteActionOnce(action, RoleOwner);
        }

        [Fact]
        public void CanCombineHaciendaAndPrivilege() {
            AddHaciendaTo(RoleOwner);
            IAction action = new TakeRandomPlantation();
            CanExecuteActionOnce(action, RoleOwner);
            action = new TakeQuarry();
            CanExecuteActionOnce(action, RoleOwner);
        }

        [Fact]
        public void CanUseHospice() {
            var player = GetPlayerWithoutPrivilege();
            var hospice = new Hospice();
            hospice.AddWorker(new Colonist());
            player.Build(hospice);
            var action = new TakePlantation {
                TileIndex = 2
            };
            CanExecuteActionOnce(action, player);
            Assert.Equal(2, player.Tiles.Count());
            Assert.NotNull(player.Tiles[1].Worker);
        }

        [Fact]
        public void TakePlantationMovesPlantationToPlayer() {
            var action = new TakePlantation {
                TileIndex = 1,
            };
            CanExecuteActionOnce(action, RoleOwner);
            Assert.Equal(2, RoleOwner.Tiles.Count());
            var player = Game.GetNextPlayerTo(RoleOwner);
            Assert.Throws<GameException>(() => CanExecuteActionOnce(action, player));
            Assert.Single(player.Tiles);
        }
        
        [Fact]
        public void PlantationsAreRefilledOnCleanUp() {
            var action = new TakePlantation {
                TileIndex = 1,
            };
            CanExecuteActionOnce(action, RoleOwner);
            Role.CleanUp();
            var player = Game.GetNextPlayerTo(RoleOwner);
            player.SelectRole(Role, Game);
            CanExecuteActionOnce(action, player);
        }

        private void AddHaciendaTo(IPlayer player) {
            Role.CleanUp();
            var hacienda = new Hacienda();
            hacienda.AddWorker(new Colonist());
            player.Build(hacienda);
            ReselectRole(player);
        }
    }
}