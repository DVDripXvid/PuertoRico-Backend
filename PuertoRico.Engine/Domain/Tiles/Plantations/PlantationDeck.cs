using System;
using System.Collections.Generic;
using System.Linq;
using PuertoRico.Engine.Domain.Player;
using PuertoRico.Engine.Domain.Resources.Goods;
using PuertoRico.Engine.Exceptions;
using PuertoRico.Engine.Shuffling;
using Toore.Shuffling;

namespace PuertoRico.Engine.Domain.Tiles.Plantations
{
    public class PlantationDeck
    {
        public IPlantation[] Drawable { get; private set; }
        private readonly List<IPlantation> _deck;
        private List<IPlantation> _discard = new List<IPlantation>();
        private readonly int _visibleCount;

        private readonly IShuffler _shuffler;
        private readonly Random _random;

        private static readonly Dictionary<Type, int> Config = new Dictionary<Type, int> {
            {typeof(IndigoPlantation), 12},
            {typeof(SugarPlantation), 11},
            {typeof(CornPlantation), 10},
            {typeof(TobaccoPlantation), 9},
            {typeof(CoffeePlantation), 8},
        };

        public PlantationDeck(int playerCount, int randomSeed) {
            _random = new Random(randomSeed);
            _shuffler = new FisherYatesShuffler(new CustomRandomWrapper(_random));
            
            _deck = new List<IPlantation>(Config.Values.Sum());
            _deck.AddRange(InitPlantationsForType<IndigoPlantation>(Config[typeof(IndigoPlantation)]));
            _deck.AddRange(InitPlantationsForType<SugarPlantation>(Config[typeof(SugarPlantation)]));
            _deck.AddRange(InitPlantationsForType<CornPlantation>(Config[typeof(CornPlantation)]));
            _deck.AddRange(InitPlantationsForType<TobaccoPlantation>(Config[typeof(TobaccoPlantation)]));
            _deck.AddRange(InitPlantationsForType<CoffeePlantation>(Config[typeof(CoffeePlantation)]));
            _deck = _deck.Shuffle(_shuffler);

            _visibleCount = playerCount + 1;
            Drawable = new IPlantation[_visibleCount];
            DiscardAndDrawNew();
        }

        public IPlantation DrawOneVisible(int index) {
            if (Drawable[index] == null) {
                throw new GameException("Plantation already drawn");
            }
            var tile = Drawable[index];
            Drawable[index] = null;
            return tile;
        }

        public IPlantation DrawRandom() {
            if (_deck.Count == 0) {
                ReUseDiscardPile();
            }
            var index = _random.Next(1, _deck.Count) - 1;
            var plantation = _deck[index];
            _deck.RemoveAt(index);
            return plantation;
        }
        
        public IPlantation DrawForType<T>() where T : IGood {
            if (_deck.Count == 0) {
                ReUseDiscardPile();
            }
            var plantation = _deck.First(p => p.CanProduce<T>());
            _deck.Remove(plantation);
            return plantation;
        }

        public void DiscardAndDrawNew() {
            foreach (var plantation in Drawable.Where(p => p != null)) {
                _discard.Add(plantation);
            }

            if (_visibleCount > _deck.Count) {
                ReUseDiscardPile();
            }
            Drawable = _deck.Take(_visibleCount).ToArray();
            _deck.RemoveRange(0, Drawable.Length);
        }

        private void ReUseDiscardPile() {
            _discard = _discard.Shuffle(_shuffler);
            _deck.AddRange(_discard);
            _discard.Clear();
        }
        
        private static IEnumerable<T> InitPlantationsForType<T>(int count) where T : IPlantation, new() {
            var plantations = new List<T>(count);
            for (var i = 0; i < count; i++) {
                plantations.Add(new T());
            }
            return plantations;
        }
    }
}