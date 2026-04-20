using HotByte.Modules.Identity.Domain.Entities;

namespace HotByte.Modules.Identity.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<AppUser>> GetAllAsync();
        Task<AppUser?> GetByIdAsync(int id);
        Task<AppUser?> GetByEmailAsync(string email);
        Task<bool> UpdateAsync(AppUser user);
        Task<bool> DeleteAsync(int id);
    }
}
