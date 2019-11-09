using System.Collections.Generic;
using PuertoRico.Engine.Actions;
using PuertoRico.Engine.Domain.Player;

namespace PuertoRico.Engine.Domain.Roles
{
    public interface IRole
    {
        string Name { get; }
        IPlayer CurrentPlayer { get; }
        int StackedDoubloons { get; }
        void AddOneDoubloon();
        void Execute(IAction action, IPlayer player);
        void OnSelect(IPlayer roleOwner);
        void CleanUp();
        HashSet<ActionType> GetAvailableActionTypes(IPlayer player);
    }
}