using System.Collections.Generic;
using System.Linq;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain.Buildings.Small;
using PuertoRico.Engine.Domain.Misc;
using PuertoRico.Engine.Domain.Player;
using PuertoRico.Engine.Exceptions;

namespace PuertoRico.Engine.Domain.Roles
{
    public class Trader : Role
    {
        private const string TradingPhase = "trade";
        private readonly TradeHouse _tradeHouse;

        public Trader(Game game) : base(game) {
            _tradeHouse = game.TradeHouse;
        }

        public override void CleanUp() {
            base.CleanUp();
            if (_tradeHouse.IsFull) {
                Game.Goods.AddRange(Game.TradeHouse.Goods);
                _tradeHouse.Goods.Clear();
            }
        }

        protected override void ExecuteInternal(IAction action, IPlayer player, string playerPhase) {
            switch (action) {
                case SellGood sellGood:
                    ExecuteSellGood(sellGood, player);
                    break;
                case EndPhase _:
                    SetPlayerPhase(player, EndedPhase);
                    break;
                default:
                    HandleUnknownPhase(playerPhase);
                    break;
            }
        }

        protected override HashSet<ActionType> GetAvailableActionTypesInternal(IPlayer player, string phase) {
            var actions = new HashSet<ActionType> {ActionType.EndPhase};
            if (phase == TradingPhase && CanSellAny(player)) {
                actions.Add(ActionType.SellGood);
            }

            return actions;
        }

        protected override string GetInitialPhase(IPlayer player) {
            return CanSellAny(player) ? TradingPhase : EndedPhase;
        }

        private bool CanSellAny(IPlayer player) {
            return !_tradeHouse.IsFull && player.Goods.Any(g => _tradeHouse.CanBeSoldBy(g.Type, player));
        }

        private void ExecuteSellGood(SellGood sellGood, IPlayer player) {
            if (!_tradeHouse.CanBeSoldBy(sellGood.GoodType, player)) {
                throw new GameException($"{sellGood.GoodType} cannot be sold");
            }

            var good = player.Goods.First(g => g.Type == sellGood.GoodType);
            player.Goods.Remove(good);
            var income = _tradeHouse.Sell(good);
            player.Doubloons += income + CalculateIncomeBonusFor(player);
            
            SetPlayerPhase(player, EndedPhase);
        }

        private int CalculateIncomeBonusFor(IPlayer player) {
            var incomeBonus = 0;
            if (HasPrivilege(player)) {
                incomeBonus += 1;
            }

            if (player.Buildings.ContainsWorkingOfType<SmallMarket>()) {
                incomeBonus += 1;
            }

            if (player.Buildings.ContainsWorkingOfType<LargeMarket>()) {
                incomeBonus += 2;
            }

            return incomeBonus;
        }
    }
}