namespace PuertoRico.Engine.Domain.Buildings.Production.Large
{
    public class CoffeeRoaster : LargeProductionBuilding
    {
        public override int Cost => 6;
        public override int VictoryPoint => 3;
        public override int WorkerCapacity => 2;
    }
}