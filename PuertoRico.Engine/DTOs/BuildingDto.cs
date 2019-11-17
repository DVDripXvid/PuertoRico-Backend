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

        public static BuildingDto Create(IBuilding building, int index) {
            return new BuildingDto {
                Name = building.Name,
                Cost = building.Cost,
                VictoryPoint = building.VictoryPoint,
                MaxDiscountByQuarry = building.MaxDiscountByQuarry,
                Index = index,
            };
        }
    }
}