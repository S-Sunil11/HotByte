using HotByte.SharedKernel;

namespace HotByte.Modules.Identity.Domain.Entities
{
    public class AuditLog : BaseEntity
    {
        public int UserID { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Resource { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? Metadata { get; set; }

        // Navigation
        public virtual AppUser? AppUser { get; set; }
    }
}
