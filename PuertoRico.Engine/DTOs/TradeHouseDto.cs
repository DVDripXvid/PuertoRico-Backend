using System.Collections.Generic;
using System.Linq;
using PuertoRico.Engine.Domain.Misc;

namespace PuertoRico.Engine.DTOs
{
    public class TradeHouseDto
    {
        public ICollection<GoodDto> Goods { get; set; }

        public static TradeHouseDto Create(TradeHouse tradeHouse) {
            var goods = tradeHouse.Goods.Select(g => GoodDto.Create(g.Type))
                .ToList();
            return new TradeHouseDto {
                Goods = goods
            };
        }
    }
}