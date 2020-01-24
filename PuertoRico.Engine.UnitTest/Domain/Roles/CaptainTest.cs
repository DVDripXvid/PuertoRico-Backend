using System.Linq;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain.Resources.Goods;
using PuertoRico.Engine.Domain.Roles;
using PuertoRico.Engine.Exceptions;
using Xunit;

namespace PuertoRico.Engine.UnitTest.Domain.Roles
{
    public class CaptainTest : BaseRoleTest<Captain>
    {
        [Fact]
        public void CannotSkipDeliver() {
            RoleOwner.Goods.Add(new Coffee());
            ReselectRole();
            var availableActionTypes = Game.GetAvailableActionTypes(RoleOwner);
            Assert.Single(availableActionTypes);
            Assert.Equal(ActionType.DeliverGoods, availableActionTypes.First());
        }

        [Fact]
        public void CanUsePrivilege() {
            RoleOwner.Goods.Add(new Coffee());
            RoleOwner.Goods.Add(new Coffee());
            ReselectRole();
            var action = new DeliverGoods {
                GoodType = GoodType.Coffee,
                ShipCapacity = 4,
            };
            CanExecuteActionOnce(action, RoleOwner);
            
            Assert.Empty(RoleOwner.Goods);
            Assert.Equal(3, RoleOwner.VictoryPointChips.Count);
        }
        
        [Fact]
        public void CannotLoadDifferentTypeOnSameShip() {
            RoleOwner.Goods.Add(new Tobacco());
            var player = GetPlayerWithoutPrivilege();
            player.Goods.Add(new Indigo());
            ReselectRole();
            var action = new DeliverGoods {
                GoodType = GoodType.Tobacco,
                ShipCapacity = 4,
            };
            CanExecuteActionOnce(action, RoleOwner);
            action.GoodType = GoodType.Indigo;
            Assert.Throws<GameException>(() => CanExecuteActionOnce(action, player));
            
            Assert.Single(player.Goods);
            Assert.Empty(player.VictoryPointChips);
        }
        
        [Fact]
        public void CannotLoadSameTypeOnDifferentShip() {
            RoleOwner.Goods.Add(new Corn());
            var player = GetPlayerWithoutPrivilege();
            player.Goods.Add(new Corn());
            ReselectRole();
            var action = new DeliverGoods {
                GoodType = GoodType.Corn,
                ShipCapacity = 4,
            };
            CanExecuteActionOnce(action, RoleOwner);
            action.ShipCapacity += 1;
            Assert.Throws<GameException>(() => CanExecuteActionOnce(action, player));

            Assert.Single(player.Goods);
            Assert.Empty(player.VictoryPointChips);
        }

        [Fact]
        public void MustLoadAsMuchAsPossible() {
            RoleOwner.Goods.Add(new Corn());
            RoleOwner.Goods.Add(new Corn());
            RoleOwner.Goods.Add(new Corn());
            RoleOwner.Goods.Add(new Corn());
            RoleOwner.Goods.Add(new Corn());
            ReselectRole();
            var action = new DeliverGoods {
                GoodType = GoodType.Corn,
                ShipCapacity = 4,
            };
            
            Assert.Throws<GameException>(() => CanExecuteActionOnce(action, RoleOwner));
            
            Assert.Equal(5, RoleOwner.Goods.Count);
            Assert.Empty(RoleOwner.VictoryPointChips);
        }
        
        [Fact]
        public void DeliversOnlyDeliverableOnes() {
            RoleOwner.Goods.Add(new Sugar());
            RoleOwner.Goods.Add(new Sugar());
            var player = GetPlayerWithoutPrivilege();
            player.Goods.Add(new Sugar());
            player.Goods.Add(new Sugar());
            player.Goods.Add(new Sugar());
            ReselectRole();
            var action = new DeliverGoods {
                GoodType = GoodType.Sugar,
                ShipCapacity = 4,
            };
            CanExecuteActionOnce(action, RoleOwner);
            CanExecuteActionOnce(action, player);
            
            Assert.Single(player.Goods);
            Assert.Equal(2, player.VictoryPointChips.Count);
        }

    }
}