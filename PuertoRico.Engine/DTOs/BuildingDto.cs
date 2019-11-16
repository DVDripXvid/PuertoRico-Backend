using PuertoRico.Engine.Domain.Buildings;

namespace PuertoRico.Engine.DTOs
{
    public class BuildingDto
    {
        public string Name { get; set; }
        public int Cost { get; set; }
        public int VictoryPoint { get; set; }
        public int MaxDiscountByQuarry { get; set; }
        public int Index { get; set; }

        public BuildingDto(IBuilding building, int index) {
            Name = building.Name;
            Cost = building.Cost;
            VictoryPoint = building.VictoryPoint;
            MaxDiscountByQuarry = building.MaxDiscountByQuarry;
            Index = index;
        }
    }
}