using PuertoRico.Engine.Domain.Buildings;

namespace PuertoRico.Engine.DTOs
{
    public class BuildingDto
    {
        public string Name { get; set; }
        public int Cost { get; set; }
        public int WorkerCapacity { get; set; }
        public int WorkerCount { get; set; }
        public int VictoryPoint { get; set; }
        public int MaxDiscountByQuarry { get; set; }
        public BuildingType Type { get; set; }
        public int Index { get; set; }

        public static BuildingDto Create(IBuilding building, int index) {
            return new BuildingDto {
                Name = building.Name,
                Cost = building.Cost,
                WorkerCapacity = building.WorkerCapacity,
                WorkerCount = building.Workers.Count,
                VictoryPoint = building.VictoryPoint,
                MaxDiscountByQuarry = building.MaxDiscountByQuarry,
                Type = building.Type,
                Index = index,
            };
        }
    }
}