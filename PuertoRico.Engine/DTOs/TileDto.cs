using PuertoRico.Engine.Domain.Tiles;

namespace PuertoRico.Engine.DTOs
{
    public class TileDto
    {
        public string Name { get; set; }
        public int Index { get; set; }

        public TileDto(ITile tile, int index) {
            Name = tile.Name;
            Index = index;
        }
    }
}