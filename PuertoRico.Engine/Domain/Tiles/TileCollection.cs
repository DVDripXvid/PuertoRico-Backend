using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PuertoRico.Engine.Domain.Resources.Goods;
using PuertoRico.Engine.Domain.Tiles.Plantations;
using PuertoRico.Engine.Exceptions;

namespace PuertoRico.Engine.Domain.Tiles
{
    public class TileCollection : IEnumerable<ITile>
    {
        private readonly List<ITile> _tiles = new List<ITile>(12);

        public IEnumerable<Quarry> Quarries => _tiles.OfType<Quarry>();
        public IEnumerable<IPlantation> Plantations => _tiles.OfType<IPlantation>();

        public ITile this[int index] => _tiles[index];

        public void Add(ITile tile) {
            if (IsFull()) {
                throw new GameException("No more tile slot");
            }

            _tiles.Add(tile);
        }

        public bool IsFull() {
            return _tiles.Count == 12;
        }

        public IEnumerator<ITile> GetEnumerator() {
            return _tiles.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}