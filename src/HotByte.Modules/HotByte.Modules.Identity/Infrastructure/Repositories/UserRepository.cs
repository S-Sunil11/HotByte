using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HotByte.Modules.Identity.Application.Interfaces;
using HotByte.Modules.Identity.Domain.Entities;

namespace HotByte.Modules.Identity.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<AppUser> _userManager;

        public UserRepository(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IEnumerable<AppUser>> GetAllAsync()
            => await _userManager.Users.ToListAsync();

        public async Task<AppUser?> GetByIdAsync(int id)
            => await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);

        public async Task<AppUser?> GetByEmailAsync(string email)
            => await _userManager.FindByEmailAsync(email);

        public async Task<bool> UpdateAsync(AppUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return false;
            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
    }
}
