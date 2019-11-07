using System;
using PuertoRico.Engine.Domain.Resources.Goods;

namespace PuertoRico.Engine.Domain.Buildings.Production
{
    public abstract class ProductionBuilding<TGoodType> : Building where TGoodType : IGood
    {
        public bool CanProduce<TGood>() where TGood : IGood {
            return typeof(TGood) == typeof(TGoodType);
        }
    }
}