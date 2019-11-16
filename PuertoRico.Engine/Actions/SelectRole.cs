namespace PuertoRico.Engine.Actions
{
    public class SelectRole : IAction
    {
        public ActionType ActionType => ActionType.SelectRole;
        public int RoleIndex { get; set; }
    }
}