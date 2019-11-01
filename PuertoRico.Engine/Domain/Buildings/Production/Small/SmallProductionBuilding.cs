namespace PuertoRico.Engine.Domain.Buildings.Production.Small
{
    public abstract class SmallProductionBuilding : ProductionBuilding
    {
        public override int WorkerCapacity => 1;
    }
}