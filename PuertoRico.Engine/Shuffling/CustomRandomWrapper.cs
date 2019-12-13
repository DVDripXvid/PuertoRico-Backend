using System;
using Toore.Shuffling;

namespace PuertoRico.Engine.Shuffling
{
    public class CustomRandomWrapper : IRandomWrapper
    {
        private readonly Random _random;
        
        public CustomRandomWrapper(Random random) {
            _random = random;
        }

        public int Next(int minValueInclusive, int maxValueExclusive)
        {
            return _random.Next(minValueInclusive, maxValueExclusive);
        }
    }
}