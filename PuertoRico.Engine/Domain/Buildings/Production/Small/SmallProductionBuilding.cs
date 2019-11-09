using PuertoRico.Engine.Domain.Resources.Goods;

namespace PuertoRico.Engine.Domain.Buildings.Production.Small
{
    public abstract class SmallProductionBuilding<TGood> : ProductionBuilding<TGood>, ISmallProductionBuilding where TGood : IGood
    {
        public override int WorkerCapacity => 1;
        public override int MaxDiscountByQuarry => 1;
    }
}