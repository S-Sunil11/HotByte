using Microsoft.AspNetCore.Identity;
using static HotByte.SharedKernel.Enums.Enum;

namespace HotByte.Modules.Identity.Domain.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public string Name { get; set; } = string.Empty;
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public UserRole Role { get; set; }
        public UserStatus Status { get; set; } = UserStatus.Active;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public virtual ICollection<AuditLog>? AuditLogs { get; set; }
    }
}
