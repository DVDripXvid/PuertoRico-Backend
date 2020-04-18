using System.Collections.Generic;
using System.Linq;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain.Buildings.Small;
using PuertoRico.Engine.Domain.Resources;
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
        public void CanShipMultipleTimes() {
            RoleOwner.Goods.Add(new Coffee());
            RoleOwner.Goods.Add(new Sugar());
            ReselectRole();
            var action = new DeliverGoods {
                GoodType = GoodType.Coffee,
                ShipCapacity = 4,
            };
            
            CanExecuteActionMultiple(action, RoleOwner);
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

        [Fact]
        public void CanStoreGoods() {
            Game.CargoShips[1].Load(new List<IGood> {new Coffee()});
            Game.CargoShips[2].Load(new List<IGood> {new Sugar()});
            for (var i = 0; i < 7; i++) {
                RoleOwner.Goods.Add(new Indigo());
            }
            
            ReselectRole();
            var deliverGoods = new DeliverGoods {
                GoodType = GoodType.Indigo,
                ShipCapacity = 4,
            };
            CanExecuteActionOnce(deliverGoods, RoleOwner);
            Assert.Equal(3, RoleOwner.Goods.Count);

            var storeGoods = new StoreGoods {
                DefaultStorage = GoodType.Indigo
            };
            CanExecuteActionOnce(storeGoods, RoleOwner);
            Assert.Equal(2, RoleOwner.Goods.Count);
            
            RoleOwner.Role.CleanUp();
            Assert.Single(RoleOwner.Goods);
        }
        
        [Fact]
        public void CanStoreGoodsWithoutDelivering() {
            Game.CargoShips[0].Load(new List<IGood> {new Tobacco()});
            Game.CargoShips[1].Load(new List<IGood> {new Coffee()});
            Game.CargoShips[2].Load(new List<IGood> {new Sugar()});
            for (var i = 0; i < 3; i++) {
                RoleOwner.Goods.Add(new Indigo());
            }
            
            ReselectRole();

            var storeGoods = new StoreGoods {
                DefaultStorage = GoodType.Indigo
            };
            CanExecuteActionOnce(storeGoods, RoleOwner);
            Assert.Equal(2, RoleOwner.Goods.Count);
            
            RoleOwner.Role.CleanUp();
            Assert.Single(RoleOwner.Goods);
        }

        [Fact]
        public void CanUseWharf() {
            var player = GetPlayerWithoutPrivilege();
            var wharf = new Wharf();
            wharf.AddWorker(new Colonist());
            player.Buildings.Add(wharf);

            var goodCount = 9;
            for (var i = 0; i < goodCount; i++) {
                player.Goods.Add(new Indigo());
            }
            
            ReselectRole();
            var useWharf = new UseWharf {
                GoodType = GoodType.Indigo
            };
            CanExecuteActionOnce(useWharf, player);
            
            Assert.Empty(player.Goods);
            Assert.Equal(goodCount, player.VictoryPointChips.Count);
        }

        [Fact]
        public void CanUseSmallWarehouse() {
            Game.CargoShips[0].Load(new List<IGood> {new Tobacco()});
            Game.CargoShips[1].Load(new List<IGood> {new Coffee()});
            Game.CargoShips[2].Load(new List<IGood> {new Sugar()});
            var smallWarehouse = new SmallWarehouse();
            smallWarehouse.AddWorker(new Colonist());
            RoleOwner.Buildings.Add(smallWarehouse);
            var goodCount = 7;
            for (var i = 0; i < goodCount; i++) {
                RoleOwner.Goods.Add(new Indigo());
            }
            
            ReselectRole();

            var storeGoods = new StoreGoods {
                SmallWarehouse = GoodType.Indigo
            };
            CanExecuteActionOnce(storeGoods, RoleOwner);
            Assert.Empty(RoleOwner.Goods);
            
            RoleOwner.Role.CleanUp();
            Assert.Equal(goodCount, RoleOwner.Goods.Count);
        }
        
        [Fact]
        public void CanUseLargeWarehouse() {
            Game.CargoShips[0].Load(new List<IGood> {new Tobacco()});
            Game.CargoShips[1].Load(new List<IGood> {new Coffee()});
            Game.CargoShips[2].Load(new List<IGood> {new Sugar()});
            var largeWarehouse = new LargeWarehouse();
            largeWarehouse.AddWorker(new Colonist());
            RoleOwner.Buildings.Add(largeWarehouse);
            var indigoCount = 5;
            for (var i = 0; i < indigoCount; i++) {
                RoleOwner.Goods.Add(new Indigo());
            }
            var cornCount = 7;
            for (var i = 0; i < cornCount; i++) {
                RoleOwner.Goods.Add(new Corn());
            }
            
            
            ReselectRole();

            var storeGoods = new StoreGoods {
                LargeWarehouse1 = GoodType.Indigo,
                LargeWarehouse2 = GoodType.Corn
            };
            CanExecuteActionOnce(storeGoods, RoleOwner);
            Assert.Empty(RoleOwner.Goods);
            
            RoleOwner.Role.CleanUp();
            Assert.Equal(indigoCount + cornCount, RoleOwner.Goods.Count);
        }

        [Fact]
        public void FullShipIsCleared() {
            var goodCount = 4;
            for (var i = 0; i < goodCount; i++) {
                RoleOwner.Goods.Add(new Indigo());
            }
            
            ReselectRole();
            var gameGoodCount = Game.Goods.Count;

            var action = new DeliverGoods {
                GoodType = GoodType.Indigo,
                ShipCapacity = goodCount
            };
            CanExecuteActionOnce(action, RoleOwner);
            
            Role.CleanUp();
            Assert.True(Game.CargoShips[0].IsEmpty());
            Assert.Equal(gameGoodCount + goodCount, Game.Goods.Count);
        }
        
        [Fact]
        public void NotFullShipIsNotCleared() {
            var goodCount = 2;
            for (var i = 0; i < goodCount; i++) {
                RoleOwner.Goods.Add(new Indigo());
            }
            
            ReselectRole();
            var gameGoodCount = Game.Goods.Count;

            var action = new DeliverGoods {
                GoodType = GoodType.Indigo,
                ShipCapacity = 4
            };
            CanExecuteActionOnce(action, RoleOwner);
            
            Role.CleanUp();
            Assert.False(Game.CargoShips[0].IsEmpty());
            Assert.Equal(gameGoodCount, Game.Goods.Count);
        }

    }
}