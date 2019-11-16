using PuertoRico.Engine.Domain.Misc;

namespace PuertoRico.Engine.DTOs
{
    public class ColonistsShipDto
    {
        public int ColonistsCount { get; set; }

        public ColonistsShipDto(ColonistsShip colonistsShip) {
            ColonistsCount = colonistsShip.ColonistCount;
        }
    }
}