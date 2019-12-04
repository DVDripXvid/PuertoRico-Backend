using PuertoRico.Engine.Actions;

namespace PuertoRico.Engine.SignalR.Commands
{
    public class GameCommand<T> : GenericGameCmd where T : IAction
    {
        public T Action { get; set; }
    }
}