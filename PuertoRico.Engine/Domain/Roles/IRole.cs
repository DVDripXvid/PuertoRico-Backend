using System.Collections.Generic;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain.Player;

namespace PuertoRico.Engine.Domain.Roles
{
    public interface IRole
    {
        string Name { get; }
        void Execute(IAction action, IPlayer player);
        void OnSelect(IPlayer roleOwner, IEnumerable<IPlayer> players);
        void CleanUp();
        HashSet<ActionType> GetAvailableActionTypes(IPlayer player);
    }
}