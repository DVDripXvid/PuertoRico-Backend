using PuertoRico.Engine.Domain.Roles;

namespace PuertoRico.Engine.DTOs
{
    public class RoleDto
    {
        public string Name { get; set; }
        public int Doubloons { get; set; }
        public int Index { get; set; }

        public static RoleDto Create(IRole role, int index) {
            if (role == null) {
                return null;
            }

            return new RoleDto {
                Doubloons = role.StackedDoubloons,
                Name = role.Name,
                Index = index,
            };
        }
    }
}