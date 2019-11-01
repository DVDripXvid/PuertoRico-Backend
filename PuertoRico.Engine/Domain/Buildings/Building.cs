namespace PuertoRico.Engine.Domain.Buildings
{
    public abstract class Building : IBuilding
    {
        public abstract int Cost { get; }
        public abstract int VictoryPoint { get; }
    }
}