﻿using System.Linq;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain;
using PuertoRico.Engine.Domain.Buildings.Small;
using PuertoRico.Engine.Domain.Resources;
using PuertoRico.Engine.Domain.Roles;
using PuertoRico.Engine.Domain.Tiles;
using Xunit;

namespace PuertoRico.Engine.Test.Domain.Roles
{
    public class MayorTest : BaseRoleTest<Mayor>
    {
        [Fact]
        public void CanSkipMoveColonist() {
            CanSkipPhase(RoleOwner, ActionType.EndRole);
        }
        
        [Fact]
        public void ColonistsMovedToPlayers() {
            Game.Players.Where(p => p != RoleOwner).ToList().ForEach(p => Assert.Single(p.IdleColonists));
            Assert.Equal(2, RoleOwner.IdleColonists.Count);
            var expectedColonistCount = GameConfig.ColonistCount[Game.PlayerCount] - (Game.PlayerCount + 1);
            Assert.Equal(expectedColonistCount, Game.Colonists.Count);
            Assert.True(Game.ColonistsShip.IsEmpty());
        }
        
        [Fact]
        public void ColonistShipRefilledOnCleanUp() {
            Role.CleanUp();
            Assert.False(Game.ColonistsShip.IsEmpty());
        }
        
        [Fact]
        public void CanMoveColonists() {
            Game.Players.ForEach(p => {
                var tile = new Quarry();
                tile.AddWorker(new Colonist());
                p.Plant(tile);
                p.Build(new SmallMarket());
            });
            var action = new MoveColonist {
                FromIndex = 0,
                ToIndex = 0,
                IsMoveFromTile = true,
                IsMoveToTile = false
            };
            Game.Players.ForEach(p => CanExecuteActionMultiple(action, p));
        }
    }
}