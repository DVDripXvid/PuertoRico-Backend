using PuertoRico.Engine.Domain.Resources.Goods;

namespace PuertoRico.Engine.Domain.Buildings.Production.Small
{
    public class SmallSugarMill : SmallProductionBuilding<Sugar>
    {
        public override int Cost => 2;
        public override int VictoryPoint => 1;
    }
}