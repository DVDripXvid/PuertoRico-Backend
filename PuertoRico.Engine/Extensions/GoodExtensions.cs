using System.Collections.Generic;
using System.Linq;
using PuertoRico.Engine.Domain.Resources.Goods;

namespace PuertoRico.Engine.Extensions
{
    public static class GoodExtensions
    {
        public static IEnumerable<IGood> OfGoodType(this IEnumerable<IGood> goods, GoodType type) {
            return goods.Where(g => g.Type == type);
        }
    }
}