using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain.Buildings.Small;
using PuertoRico.Engine.Domain.Resources;
using PuertoRico.Engine.Domain.Resources.Goods;
using PuertoRico.Engine.Domain.Roles;
using PuertoRico.Engine.Exceptions;
using Xunit;

namespace PuertoRico.Engine.Test.Domain.Roles
{
    public class TraderTest : BaseRoleTest<Trader>
    {
        [Fact]
        public void CanSkipTrade() {
            RoleOwner.Goods.Add(new Coffee());
            ReselectRole();
            CanSkipPhase(RoleOwner, ActionType.EndRole);
        }

        [Fact]
        public void CanUsePrivilege() {
            RoleOwner.Goods.Add(new Coffee());
            ReselectRole();
            var action = new SellGood {
                GoodType = GoodType.Coffee
            };
            CanExecuteActionOnce(action, RoleOwner);

            Assert.Equal(DoubloonsOnRole + 5, RoleOwner.Doubloons);
        }

        [Fact]
        public void SameGoodCannotBeSold() {
            var player = GetPlayerWithoutPrivilege();
            RoleOwner.Goods.Add(new Coffee());
            player.Goods.Add(new Coffee());
            player.Goods.Add(new Tobacco());
            ReselectRole();
            var action = new SellGood {
                GoodType = GoodType.Coffee
            };
            CanExecuteActionOnce(action, RoleOwner);
            Assert.Throws<GameException>(() => CanExecuteActionOnce(action, player));

            Assert.Equal(0, player.Doubloons);
        }

        [Fact]
        public void CanUseOffice() {
            var player = GetPlayerWithoutPrivilege();
            RoleOwner.Goods.Add(new Coffee());
            player.Goods.Add(new Coffee());
            ReselectRole();
            var office = new Office();
            office.AddWorker(new Colonist());
            player.Build(office);
            var action = new SellGood {
                GoodType = GoodType.Coffee
            };
            CanExecuteActionOnce(action, RoleOwner);
            CanExecuteActionOnce(action, player);

            Assert.Equal(4, player.Doubloons);
        }

        [Fact]
        public void CanUseSmallMarket() {
            var player = GetPlayerWithoutPrivilege();
            player.Goods.Add(new Coffee());
            var smallMarket = new SmallMarket();
            smallMarket.AddWorker(new Colonist());
            player.Build(smallMarket);
            ReselectRole();
            var action = new SellGood {
                GoodType = GoodType.Coffee
            };
            CanExecuteActionOnce(action, player);
            
            Assert.Equal(5, player.Doubloons);
        }
        
        [Fact]
        public void CanUseLargeMarket() {
            var player = GetPlayerWithoutPrivilege();
            player.Goods.Add(new Coffee());
            var largeMarket = new LargeMarket();
            largeMarket.AddWorker(new Colonist());
            player.Build(largeMarket);
            ReselectRole();
            var action = new SellGood {
                GoodType = GoodType.Coffee
            };
            CanExecuteActionOnce(action, player);
            
            Assert.Equal(6, player.Doubloons);
        }
        
        [Fact]
        public void CanCombinePrivilegeAndSmallMarketAndLargeMarket() {
            var player = RoleOwner;
            player.Goods.Add(new Coffee());
            var largeMarket = new LargeMarket();
            largeMarket.AddWorker(new Colonist());
            player.Build(largeMarket);
            var smallMarket = new SmallMarket();
            smallMarket.AddWorker(new Colonist());
            player.Build(smallMarket);
            ReselectRole();
            var action = new SellGood {
                GoodType = GoodType.Coffee
            };
            CanExecuteActionOnce(action, player);
            
            Assert.Equal(DoubloonsOnRole + 8, player.Doubloons);
        }
    }
}