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

namespace PuertoRico.Engine.Test.Domain.Roles
{
    public class BuilderTest : BaseRoleTest<Builder>
    {

        [Fact]
        public void CanSkipAction() {
            var actions = _roleOwner.GetAvailableActionTypes();
            Assert.Contains(ActionType.EndPhase, actions);
            
            var action = new EndPhase();
            _role.Execute(action, _roleOwner);

            actions = _roleOwner.GetAvailableActionTypes();
            Assert.Single(actions);
            Assert.Contains(ActionType.EndRole, actions);
        }

        [Fact]
        public void CanBuildOnlyOnce() {
            _roleOwner.Doubloons = 21;
            var actions = _roleOwner.GetAvailableActionTypes();
            Assert.Contains(ActionType.Build, actions);
            
            var action = new Build {
                BuildingIndex = 0,
            };
            _role.Execute(action, _roleOwner);
            
            actions = _roleOwner.GetAvailableActionTypes();
            Assert.Single(actions);
            Assert.Contains(ActionType.EndRole, actions);
        }

        [Fact]
        public void CanUsePrivilege() {
            const int playerFunds = 21;
            _roleOwner.Doubloons = playerFunds;

            var building = _game.Buildings.First();
            var action = new Build {
                BuildingIndex = 0,
            };
            _role.Execute(action, _roleOwner);
            
            Assert.Equal(playerFunds - building.Cost + 1, _roleOwner.Doubloons);
        }

        [Fact]
        public void MovesBuildingToUser() {
            _roleOwner.Doubloons = 21;

            var building = _game.Buildings.First();
            var action = new Build {
                BuildingIndex = 0,
            };
            _role.Execute(action, _roleOwner);
            
            Assert.Contains(building, _roleOwner.Buildings);
            Assert.DoesNotContain(building, _game.Buildings);
        }

        [Fact]
        public void CanUseUniversity() {
            _roleOwner.Doubloons = 21;
            var university = new University();
            university.AddWorker(new Colonist());
            _roleOwner.Build(university);

            var building = _game.Buildings.First();
            var action = new Build {
                BuildingIndex = 0,
            };
            _role.Execute(action, _roleOwner);

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
            CanUseQuarry_ToBuild<Hacienda>(2, _roleOwner, 2);
        }

        [Fact]
        public void DiscountCanNotExceedCost() {
            CanUseQuarry_ToBuild<SmallIndigoPlant>(1, _roleOwner, 2);
        }

        private void CanUseQuarry_ToBuild<T>(int expectedDiscount, IPlayer player, int quarryCount) where T: IBuilding{
            const int playerFunds = 21;
            player.Doubloons = playerFunds;
            for (var i = 0; i < quarryCount; i++) {
                var quarry = new Quarry();
                quarry.AddWorker(new Colonist());
                player.Plant(quarry);
            }

            var building = _game.Buildings.OfType<T>().First();
            var index = _game.Buildings.IndexOf(building);
            var action = new Build {
                BuildingIndex = index
            };
            _role.Execute(action, player);
            
            Assert.Equal(playerFunds - building.Cost + expectedDiscount, player.Doubloons);
        }

        private IPlayer GetPlayerWithoutPrivilege() {
            return _game.Players.Find(p => p != _roleOwner);
        }
    }
}