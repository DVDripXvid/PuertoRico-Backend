using System;
using System.Collections.Generic;
using System.Linq;
using PuertoRico.Engine.Domain.Resources.Goods;
using Toore.Shuffling;

namespace PuertoRico.Engine.Domain.Tiles.Plantations
{
    public class PlantationDeck
    {
        public IPlantation[] Drawable { get; private set; }
        private readonly List<IPlantation> _deck;
        private readonly int _visibleCount;

        private static readonly IShuffler Shuffler = new FisherYatesShuffler(new RandomWrapper());
        private readonly Random _random = new Random();

        private static readonly Dictionary<Type, int> Config = new Dictionary<Type, int> {
            {typeof(IndigoPlantation), 12},
            {typeof(SugarPlantation), 11},
            {typeof(CornPlantation), 10},
            {typeof(TobaccoPlantation), 9},
            {typeof(CoffeePlantation), 8},
        };

        public PlantationDeck(int playerCount) {
            _deck = new List<IPlantation>(Config.Values.Sum());
            _deck.AddRange(InitPlantationsForType<IndigoPlantation>(Config[typeof(IndigoPlantation)]));
            _deck.AddRange(InitPlantationsForType<SugarPlantation>(Config[typeof(SugarPlantation)]));
            _deck.AddRange(InitPlantationsForType<CornPlantation>(Config[typeof(CornPlantation)]));
            _deck.AddRange(InitPlantationsForType<TobaccoPlantation>(Config[typeof(TobaccoPlantation)]));
            _deck.AddRange(InitPlantationsForType<CoffeePlantation>(Config[typeof(CoffeePlantation)]));
            _deck.Shuffle(Shuffler);

            _visibleCount = playerCount + 1;
            Drawable = new IPlantation[_visibleCount];
            DiscardAndDrawNew();
        }

        public IPlantation DrawOneVisible(int index) {
            var tile = Drawable[index];
            Drawable[index] = null;
            return tile;
        }

        public IPlantation DrawRandom() {
            var index = _random.Next(1, _deck.Count) - 1;
            var plantation = _deck[index];
            _deck.RemoveAt(index);
            return plantation;
        }

        public void DiscardAndDrawNew() {
            Drawable = _deck.Take(_visibleCount).ToArray();
            _deck.RemoveRange(0, _visibleCount);
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