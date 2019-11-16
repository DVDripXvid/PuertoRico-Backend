using PuertoRico.Engine.Domain.Resources.Goods;

namespace PuertoRico.Engine.DTOs
{
    public class GoodDto
    {
        public GoodType Type { get; set; }

        public GoodDto(GoodType goodType) {
            Type = goodType;
        }
    }
}