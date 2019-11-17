using PuertoRico.Engine.Domain.Resources.Goods;

namespace PuertoRico.Engine.DTOs
{
    public class GoodDto
    {
        public GoodType Type { get; set; }

        public static GoodDto Create(GoodType goodType) {
            return new GoodDto {
                Type = goodType
            };
        }
    }
}