using Newtonsoft.Json;

namespace PuertoRico.Engine.Actions
{
    public interface IAction
    {
        [JsonIgnore] ActionType ActionType { get; }
    }
}