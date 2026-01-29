namespace SLM.Core.Models
{
    public class AuditLog : BaseEntity
    {
        public int? UserId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public int? EntityId { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;

        // Navigation properties
        public User? User { get; set; }
    }
}