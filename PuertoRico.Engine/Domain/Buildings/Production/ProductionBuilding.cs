using System;
using PuertoRico.Engine.Domain.Resources.Goods;

namespace PuertoRico.Engine.Domain.Buildings.Production
{
    public abstract class ProductionBuilding<TGoodType> : Building, IProductionBuilding where TGoodType : IGood
    {
        public override BuildingType Type => BuildingType.Production;

        public bool CanProduce<TGood>() where TGood : IGood {
            return typeof(TGood) == typeof(TGoodType);
        }
    }
}