using System.Linq;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain.Buildings;
using PuertoRico.Engine.Domain.Buildings.Production.Small;
using PuertoRico.Engine.Domain.Buildings.Small;
using PuertoRico.Engine.Domain.Player;
using PuertoRico.Engine.Domain.Resources;
using PuertoRico.Engine.Domain.Roles;
using PuertoRico.Engine.Domain.Tiles;
using Xunit;

namespace PuertoRico.Engine.UnitTest.Domain.Roles
{
    public class BuilderTest : BaseRoleTest<Builder>
    {
        
        [Fact]
        public void CanSkipBuild() {
            CanSkipPhase(RoleOwner, ActionType.EndRole);
        }

        [Fact]
        public void CanBuildOnlyOnce() {
            RoleOwner.Doubloons = 21;
            var action = new Build {
                BuildingIndex = 0,
            };
            CanExecuteActionOnce(action, RoleOwner);
        }

        [Fact]
        public void CanUsePrivilege() {
            const int playerFunds = 21;
            RoleOwner.Doubloons = playerFunds;

            var building = Game.Buildings.First();
            var action = new Build {
                BuildingIndex = 0,
            };
            Role.Execute(action, RoleOwner);
            
            Assert.Equal(playerFunds - building.Cost + 1, RoleOwner.Doubloons);
        }

        [Fact]
        public void MovesBuildingToUser() {
            RoleOwner.Doubloons = 21;

            var building = Game.Buildings.First();
            var action = new Build {
                BuildingIndex = 0,
            };
            Role.Execute(action, RoleOwner);
            
            Assert.Contains(building, RoleOwner.Buildings);
            Assert.DoesNotContain(building, Game.Buildings);
        }

        [Fact]
        public void CanUseUniversity() {
            RoleOwner.Doubloons = 21;
            var university = new University();
            university.AddWorker(new Colonist());
            RoleOwner.Build(university);

            var building = Game.Buildings.First();
            var action = new Build {
                BuildingIndex = 0,
            };
            Role.Execute(action, RoleOwner);

            Assert.Single(building.Workers);
        }

        [Fact]
        public void CanUseQuarry_WithMaxDiscount() {
            var player = GetPlayerWithoutPrivilege();
            CanUseQuarry_ToBuild<Hospice>(2, player, 2);
        }
        
        [Fact]
        public void CanUseQuarry_WithPartialDiscount() {
            var player = GetPlayerWithoutPrivilege();
            CanUseQuarry_ToBuild<Hacienda>(1, player, 2);
        }

        [Fact]
        public void CombinePrivilegeWithQuarry() {
            CanUseQuarry_ToBuild<Hacienda>(2, RoleOwner, 2);
        }

        [Fact]
        public void DiscountCanNotExceedCost() {
            CanUseQuarry_ToBuild<SmallIndigoPlant>(1, RoleOwner, 2);
        }

        private void CanUseQuarry_ToBuild<T>(int expectedDiscount, IPlayer player, int quarryCount) where T: IBuilding{
            const int playerFunds = 21;
            player.Doubloons = playerFunds;
            for (var i = 0; i < quarryCount; i++) {
                var quarry = new Quarry();
                quarry.AddWorker(new Colonist());
                player.Plant(quarry);
            }

            var building = Game.Buildings.OfType<T>().First();
            var index = Game.Buildings.IndexOf(building);
            var action = new Build {
                BuildingIndex = index
            };
            Role.Execute(action, player);
            
            Assert.Equal(playerFunds - building.Cost + expectedDiscount, player.Doubloons);
        }
    }
}