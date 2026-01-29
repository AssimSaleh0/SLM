using SLM.Core.Enums;

namespace SLM.Core.Models
{
    public class Role : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}