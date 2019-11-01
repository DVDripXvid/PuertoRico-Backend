using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PuertoRico.Engine.Domain.Tiles
{
    public class TilesCollection : IEnumerable<ITile>
    {
        private readonly List<ITile> _tiles;

        public IEnumerable<Quarry> Quarries => _tiles.OfType<Quarry>();

        public TilesCollection() {
            _tiles = new List<ITile>();
        }

        public void Add(ITile tile) {
            _tiles.Add(tile);
        }

        public IEnumerator<ITile> GetEnumerator() {
            return _tiles.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}