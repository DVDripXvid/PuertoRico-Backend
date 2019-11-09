using PuertoRico.Engine.Domain.Resources.Goods;

namespace PuertoRico.Engine.Domain.Buildings.Production.Large
{
    public abstract class LargeProductionBuilding<TGood> : ProductionBuilding<TGood>, ILargeProductionBuilding where TGood : IGood
    {
        public override int WorkerCapacity => 3;
    }
}