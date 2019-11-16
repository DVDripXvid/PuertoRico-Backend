using System.Collections.Generic;
using System.Linq;
using PuertoRico.Engine.Domain.Player;
using PuertoRico.Engine.Domain.Resources.Goods;
using PuertoRico.Engine.Exceptions;

namespace PuertoRico.Engine.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IGood> OfGoodType(this IEnumerable<IGood> goods, GoodType type) {
            return goods.Where(g => g.Type == type);
        }
        
        public static IPlayer WithUserId(this IEnumerable<IPlayer> players, string userId) {
            return players.SingleOrDefault(p => p.UserId == userId) ?? throw new GameException($"Player not found with user id = " + userId);
        }
    }
}