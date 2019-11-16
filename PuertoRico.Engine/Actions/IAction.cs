using System.Text.Json.Serialization;
using PuertoRico.Engine.SignalR;

namespace PuertoRico.Engine.Actions
{
    public interface IAction
    {
        ActionType ActionType { get; }
    }
}