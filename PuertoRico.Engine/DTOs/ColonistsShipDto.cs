using PuertoRico.Engine.Domain.Misc;

namespace PuertoRico.Engine.DTOs
{
    public class ColonistsShipDto
    {
        public int ColonistsCount { get; set; }

        public static ColonistsShipDto Create(ColonistsShip colonistsShip) {
            return new ColonistsShipDto {
                ColonistsCount = colonistsShip.ColonistCount
            };
        }
    }
}