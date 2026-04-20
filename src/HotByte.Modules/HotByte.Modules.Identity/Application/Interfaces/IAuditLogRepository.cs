using HotByte.Modules.Identity.Domain.Entities;

namespace HotByte.Modules.Identity.Application.Interfaces
{
    public interface IAuditLogRepository
    {
        Task CreateAsync(AuditLog log);
        Task<IEnumerable<AuditLog>> GetAllAsync();
        Task<IEnumerable<AuditLog>> GetByUserIdAsync(int userId);
    }
}
