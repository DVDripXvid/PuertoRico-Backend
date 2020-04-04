namespace PuertoRico.Engine.Domain.Buildings.Small
{
    public abstract class SmallBuilding : Building
    {
        public override int WorkerCapacity => 1;
        public override BuildingType Type => BuildingType.Small;
    }
}