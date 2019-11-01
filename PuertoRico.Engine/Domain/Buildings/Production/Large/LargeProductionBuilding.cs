namespace PuertoRico.Engine.Domain.Buildings.Production.Large
{
    public abstract class LargeProductionBuilding : ProductionBuilding
    {
        public override int WorkerCapacity => 3;
    }
}