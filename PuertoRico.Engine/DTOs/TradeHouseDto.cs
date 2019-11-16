using System.Collections.Generic;
using System.Linq;
using PuertoRico.Engine.Domain.Misc;

namespace PuertoRico.Engine.DTOs
{
    public class TradeHouseDto
    {
        public ICollection<GoodDto> Goods { get; set; }

        public TradeHouseDto(TradeHouse tradeHouse) {
            Goods = tradeHouse.Goods.
                Select(g => new GoodDto(g.Type))
                .ToList();
        }
    }
}