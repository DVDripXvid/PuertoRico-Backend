using Newtonsoft.Json;
using PuertoRico.Engine.Domain.Resources;

namespace PuertoRico.Engine.Domain.Tiles
{
    public interface ITile
    {
        [JsonIgnore]
        string Name { get; }
        [JsonIgnore]
        Colonist Worker { get; }
        void AddWorker(Colonist worker);
        Colonist RemoveWorker();
    }
}