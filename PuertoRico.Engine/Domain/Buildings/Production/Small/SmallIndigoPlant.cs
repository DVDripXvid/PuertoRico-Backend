using PuertoRico.Engine.Domain.Resources.Goods;

namespace PuertoRico.Engine.Domain.Buildings.Production.Small
{
    public class SmallIndigoPlant : SmallProductionBuilding<Indigo>
    {
        public override int Cost => 1;
        public override int VictoryPoint => 1;
    }
}