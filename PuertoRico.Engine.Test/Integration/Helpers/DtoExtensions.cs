using System.Collections.Generic;
using System.Linq;
using PuertoRico.Engine.DTOs;

namespace PuertoRico.Engine.Test.Integration.Helpers
{
    public static class DtoExtensions
    {
        public static int IndexOf(this IEnumerable<BuildingDto> buildings, string name) {
            return buildings.First(b => b.Name == name).Index;
        }
    }
}