using System.Collections.Generic;
using System.Linq;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain.Buildings.Small;
using PuertoRico.Engine.Domain.Player;
using PuertoRico.Engine.Domain.Resources.Goods;
using PuertoRico.Engine.Exceptions;

namespace PuertoRico.Engine.Domain.Roles
{
    public class Captain : Role
    {
        private const string DeliverPhase = "deliver";
        private const string StorePhase = "store";
        private readonly Dictionary<string, bool> _isWharfUsedBy;
        private bool _isPrivilegeUsed;
        private readonly Dictionary<string, IGood> _defaultStore = new Dictionary<string, IGood>();

        public Captain(Game game) : base(game) {
            _isWharfUsedBy = new Dictionary<string, bool>(Game.PlayerCount);
            Game.Players.ForEach(p => _isWharfUsedBy[p.UserId] = false);
        }

        public override void CleanUp() {
            base.CleanUp();
            _isPrivilegeUsed = false;
            foreach (var userId in _isWharfUsedBy.Keys.ToList()) {
                _isWharfUsedBy[userId] = false;
            }

            Game.Players.ForEach(p => {
                p.Goods.Clear();
                ReleaseStoredGoods(p);
            });
        }

        protected override void ExecuteInternal(IAction action, IPlayer player, string playerPhase) {
            switch (action) {
                case DeliverGoods deliverGoods:
                    ExecuteDeliverGoods(deliverGoods, player);
                    break;
                case UseWharf useWharf:
                    ExecuteUseWharf(useWharf, player);
                    break;
                case StoreGoods storeGoods:
                    ExecuteStoreGoods(storeGoods, player);
                    break;
                case EndPhase _:
                    SetPlayerPhase(player, EndedPhase);
                    break;
                default:
                    HandleUnsupportedAction(action);
                    break;
            }
        }

        protected override HashSet<ActionType> GetAvailableActionTypesInternal(IPlayer player, string phase) {
            var actions = new HashSet<ActionType>();
            switch (phase) {
                case DeliverPhase:
                    if (IsAbleToUseCargoShip(player)) {
                        actions.Add(ActionType.DeliverGoods);
                    }

                    if (IsAbleToUseWharf(player)) {
                        actions.Add(ActionType.UseWharf);
                    }

                    break;
                case StorePhase:
                    actions.Add(ActionType.EndPhase);
                    if (player.Goods.Count > 0) {
                        actions.Add(ActionType.StoreGoods);
                    }

                    break;
                default:
                    HandleUnknownPhase(phase);
                    break;
            }

            return actions;
        }

        protected override string GetInitialPhase(IPlayer player) {
            return (IsAbleToUseCargoShip(player) || IsAbleToUseWharf(player)) ? DeliverPhase : EndedPhase;
        }

        private void ExecuteDeliverGoods(DeliverGoods deliverGoods, IPlayer player) {
            var cargo = Game.CargoShips.FirstOrDefault(c => c.Capacity == deliverGoods.ShipCapacity);
            if (cargo == null) {
                throw new GameException($"Cargo ship with capacity {deliverGoods.ShipCapacity} does not exist");
            }

            var goodsToDeliver = player.Goods.Where(g => g.Type == deliverGoods.GoodType).ToList();

            if (goodsToDeliver.Count > 0
                || !cargo.CanBeLoaded(deliverGoods.GoodType)) {
                throw new GameException($"{deliverGoods.GoodType} cannot be loaded");
            }

            var deliveredCount = cargo.Load(goodsToDeliver);
            var deliveredGoods = goodsToDeliver.Take(deliveredCount).ToList();
            DoDeliver(deliveredGoods, player);

            if (IsAbleToUseCargoShip(player) && IsAbleToUseWharf(player)) {
                Game.MoveToNextPlayer();
            }
            else if (player.Goods.Count > 0) {
                SetPlayerPhase(player, StorePhase);
            }
            else {
                SetPlayerPhase(player, EndedPhase);
            }
        }

        private void ExecuteUseWharf(UseWharf useWharf, IPlayer player) {
            _isWharfUsedBy[player.UserId] = true;
            var deliveredGoods = player.Goods.Where(g => g.Type == useWharf.GoodType).ToList();
            DoDeliver(deliveredGoods, player);
            if (IsAbleToUseCargoShip(player)) {
                Game.MoveToNextPlayer();
            }
            else if (player.Goods.Count > 0) {
                SetPlayerPhase(player, StorePhase);
            }
            else {
                SetPlayerPhase(player, EndedPhase);
            }
        }

        private void ExecuteStoreGoods(StoreGoods storeGoods, IPlayer player) {
            var largeWarehouse = GetLargeWarehouseOrDefault(player);
            if (storeGoods.LargeWarehouse1.HasValue) {
                largeWarehouse?.StoreGoods(storeGoods.LargeWarehouse1.Value, player);
            }

            if (storeGoods.LargeWarehouse2.HasValue) {
                largeWarehouse?.StoreGoods(storeGoods.LargeWarehouse2.Value, player);
            }

            var smallWarehouse = GetSmallWarehouseOrDefault(player);
            if (storeGoods.SmallWarehouse.HasValue) {
                smallWarehouse?.StoreGoods(storeGoods.SmallWarehouse.Value, player);
            }

            if (storeGoods.DefaultStorage.HasValue) {
                _defaultStore[player.UserId] =
                    player.Goods.FirstOrDefault(g => g.Type == storeGoods.DefaultStorage.Value);
            }

            SetPlayerPhase(player, EndedPhase);
        }

        private bool IsAbleToUseCargoShip(IPlayer player) {
            return player.Goods.Select(g => g.Type).Distinct().Any(CanBeLoaded);
        }

        private bool IsAbleToUseWharf(IPlayer player) {
            return player.Buildings.ContainsWorkingOfType<Wharf>() && !_isWharfUsedBy[player.UserId];
        }

        private bool CanBeLoaded(GoodType goodType) {
            return Game.CargoShips.Exists(ship => ship.CanBeLoaded(goodType));
        }

        private void ReleaseStoredGoods(IPlayer player) {
            if (_defaultStore.ContainsKey(player.UserId) && _defaultStore[player.UserId] != null) {
                player.Goods.Add(_defaultStore[player.UserId]);
            }

            _defaultStore.Clear();

            var largeWarehouse = GetLargeWarehouseOrDefault(player);
            if (largeWarehouse != null) {
                player.Goods.AddRange(largeWarehouse.ReleaseGoods());
            }

            var smallWarehouse = GetSmallWarehouseOrDefault(player);
            if (smallWarehouse != null) {
                player.Goods.AddRange(smallWarehouse.ReleaseGoods());
            }
        }

        private void DoDeliver(ICollection<IGood> deliveredGoods, IPlayer player) {
            var deliveredCount = deliveredGoods.Count;
            player.Goods.RemoveAll(deliveredGoods.Contains);
            Game.Goods.AddRange(deliveredGoods);
            var vpCount = deliveredCount;
            if (HasPrivilege(player) == !_isPrivilegeUsed) {
                _isPrivilegeUsed = true;
                vpCount += 1;
            }

            if (player.Buildings.ContainsWorkingOfType<Harbor>()) {
                vpCount += 1;
            }

            var vpList = Game.VictoryPointChips.Take(vpCount).ToList();
            player.VictoryPointChips.AddRange(vpList);
            Game.VictoryPointChips.RemoveRange(0, vpList.Count);
        }

        private static LargeWarehouse GetLargeWarehouseOrDefault(IPlayer player) {
            return player.Buildings.FirstOrDefault(b => b is LargeWarehouse) as LargeWarehouse;
        }

        private static SmallWarehouse GetSmallWarehouseOrDefault(IPlayer player) {
            return player.Buildings.FirstOrDefault(b => b is SmallWarehouse) as SmallWarehouse;
        }
    }
}