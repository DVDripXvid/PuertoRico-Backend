using System.Collections.Generic;

namespace PuertoRico.Engine.Domain
{
    public static class GameConfig
    {
        public static readonly Dictionary<int, int> ColonistCount = new Dictionary<int, int> {
            {3, 55},
            {4, 75},
            {5, 95}
        };

        public const int MinPlayer = 3;
        public const int MaxPlayer = 5;

        public const int QuarryCount = 8;

        public const int IndigoCount = 11;
        public const int SugarCount = 11;
        public const int CornCount = 10;
        public const int TobaccoCount = 9;
        public const int CoffeeCount = 9;
    }
}