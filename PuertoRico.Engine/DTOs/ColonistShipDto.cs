using PuertoRico.Engine.Domain.Misc;

namespace PuertoRico.Engine.DTOs
{
    public class ColonistShipDto
    {
        public int ColonistCount { get; set; }

        public static ColonistShipDto Create(ColonistShip colonistShip) {
            return new ColonistShipDto {
                ColonistCount = colonistShip.ColonistCount
            };
        }
    }
}