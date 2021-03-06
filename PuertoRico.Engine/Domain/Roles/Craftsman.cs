﻿using System;
using System.Collections.Generic;
using System.Linq;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain.Buildings.Small;
using PuertoRico.Engine.Domain.Player;
using PuertoRico.Engine.Domain.Resources.Goods;
using PuertoRico.Engine.Exceptions;

namespace PuertoRico.Engine.Domain.Roles
{
    public class Craftsman : Role
    
    {
        private const string BonusProductionPhase = "production";
        private Dictionary<GoodType, bool> _isProducedByRoleOwner;

        public Craftsman(Game game) : base(game) { }

        public override void OnSelect(IPlayer roleOwner) {
            base.OnSelect(roleOwner);
            Game.ForEachPlayerStartWith(roleOwner, p => {
                var producedIndigo = Produce<Indigo>(p);
                var producedSugar = Produce<Sugar>(p);
                var producedTobacco = Produce<Tobacco>(p);
                var producedCoffee = Produce<Coffee>(p);
                var producedCorn = Produce<Corn>(p);
                var isProduced = new Dictionary<GoodType, bool> {
                    {GoodType.Indigo, producedIndigo > 0},
                    {GoodType.Sugar, producedSugar > 0},
                    {GoodType.Tobacco, producedTobacco > 0},
                    {GoodType.Coffee, producedCoffee > 0},
                    {GoodType.Corn, producedCorn > 0},
                };
                if (p.Buildings.ContainsWorkingOfType<Factory>()) {
                    var producedTypeCount = isProduced.Values.Count(v => v);
                    p.Doubloons += Factory.BonusForProduction[producedTypeCount];
                }
                if (HasPrivilege(p)) {
                    _isProducedByRoleOwner = isProduced;
                }
            });
        }

        protected override void ExecuteInternal(IAction action, IPlayer player, string playerPhase) {
            switch (action) {
                case BonusProduction bonusProduction:
                    ExecuteBonusProduction(bonusProduction, player);
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
            if (phase == BonusProductionPhase) {
                actions.Add(ActionType.BonusProduction);
            }

            return actions;
        }

        protected override string GetInitialPhase(IPlayer player) {
            return HasPrivilege(player) ? BonusProductionPhase : EndedPhase;
        }

        private void ExecuteBonusProduction(BonusProduction bonusProduction, IPlayer player) {
            if (!_isProducedByRoleOwner[bonusProduction.GoodType]) {
                throw new GameException($"{bonusProduction.GoodType} was not produced");
            }

            var zeroOrOneGoodList = Game.Goods.Where(g => g.Type == bonusProduction.GoodType).Take(1).ToList();
            Game.Goods.RemoveAll(zeroOrOneGoodList.Contains);
            player.AddGoods(zeroOrOneGoodList);
            SetPlayerPhase(player, EndedPhase);
        }

        private int Produce<TGood>(IPlayer player) where TGood : IGood {
            var countOfWorkingPlantation = GetCountOfWorkingPlantationOfType<TGood>(player);
            var sumOfBuildingWorkers = typeof(TGood) == typeof(Corn)
                ? countOfWorkingPlantation
                : GetSumOfBuildingWorkersOfType<TGood>(player);

            var producedBarrels =
                Game.Goods.OfType<TGood>().Take(Math.Min(sumOfBuildingWorkers, countOfWorkingPlantation))
                    .Select(g => g as IGood).ToList();
            Game.Goods.RemoveAll(producedBarrels.Contains);
            player.AddGoods(producedBarrels);
            return producedBarrels.Count;
        }

        private static int GetSumOfBuildingWorkersOfType<TGood>(IPlayer player) where TGood : IGood {
            return player.Buildings.ProductionBuildings
                .Where(b => b.CanProduce<TGood>())
                .Sum(b => b.Workers.Count);
        }

        private static int GetCountOfWorkingPlantationOfType<TGood>(IPlayer player) where TGood : IGood {
            return player.Tiles.Plantations
                .Where(t => t.CanProduce<TGood>())
                .Count(t => t.Worker != null);
        }
    }
}