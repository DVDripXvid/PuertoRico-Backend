using PuertoRico.Engine.Domain.Roles;

namespace PuertoRico.Engine.DTOs
{
    public class RoleDto
    {
        public string Name { get; set; }
        public int Doubloons { get; set; }
        public int Index { get; set; }

        public RoleDto(IRole role, int index) {
            Name = role.Name;
            Doubloons = role.StackedDoubloons;
            Index = index;
        }
    }
}