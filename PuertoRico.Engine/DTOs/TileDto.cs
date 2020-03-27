using PuertoRico.Engine.Domain.Tiles;

namespace PuertoRico.Engine.DTOs
{
    public class TileDto
    {
        public string Name { get; set; }
        public bool HasWorker { get; set; }
        public int Index { get; set; }

        public static TileDto Create(ITile tile, int index) {
            return new TileDto {
                Name = tile?.Name,
                HasWorker = tile?.Worker != null,
                Index = index,
            };
        }
    }
}