using Microsoft.EntityFrameworkCore;
using HotByte.Modules.Identity.Application.Interfaces;
using HotByte.Modules.Identity.Domain.Entities;

namespace HotByte.Modules.Identity.Infrastructure.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly IdentityDbContext _context;

        public AuditLogRepository(IdentityDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(AuditLog log)
        {
            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetAllAsync()
        {
            return await _context.AuditLogs
                .Include(a => a.AppUser)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(int userId)
        {
            return await _context.AuditLogs
                .Include(a => a.AppUser)
                .Where(a => a.UserID == userId)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }
    }
}
