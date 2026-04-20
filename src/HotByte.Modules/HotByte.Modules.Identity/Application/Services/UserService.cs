using Microsoft.AspNetCore.Identity;
using HotByte.Modules.Identity.Application.DTOs;
using HotByte.Modules.Identity.Application.Interfaces;
using HotByte.Modules.Identity.Application.Mappings;
using HotByte.Modules.Identity.Domain.Entities;
using HotByte.SharedKernel.Interfaces;
using static HotByte.SharedKernel.Enums.Enum;

namespace HotByte.Modules.Identity.Application.Services
{
    public class UserService : IUserService, IUserPublicService
    {
        private readonly IUserRepository _userRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly IAuditLogRepository _auditLog;

        public UserService(
            IUserRepository userRepo,
            UserManager<AppUser> userManager,
            IAuditLogRepository auditLog)
        {
            _userRepo = userRepo;
            _userManager = userManager;
            _auditLog = auditLog;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepo.GetAllAsync();
            return IdentityMapper.ToDtoList(users);
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            return user == null ? null : IdentityMapper.ToDto(user);
        }

        // Customer self-registration — role is always forced to Customer
        public async Task<UserDto?> RegisterCustomerAsync(RegisterCustomerDto dto)
        {
            var user = new AppUser
            {
                Name = dto.Name,
                UserName = dto.Email,
                Email = dto.Email,
                PhoneNumber = dto.Phone,
                Gender = dto.Gender,
                Address = dto.Address,
                Role = UserRole.Customer,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return null;

            await _userManager.AddToRoleAsync(user, UserRole.Customer.ToString());

            await _auditLog.CreateAsync(new AuditLog
            {
                UserID = user.Id,
                Action = "REGISTER",
                Resource = "User",
                Metadata = $"Customer {user.Email} self-registered"
            });

            return IdentityMapper.ToDto(user);
        }

        // Used by Restaurant module when Admin creates a restaurant + owner in one call
        public async Task<int> CreateRestaurantOwnerAccountAsync(
            string name, string email, string password, string? phone)
        {
            var user = new AppUser
            {
                Name = name,
                UserName = email,
                Email = email,
                PhoneNumber = phone,
                Role = UserRole.RestaurantOwner,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Owner account creation failed: {errors}");
            }

            await _userManager.AddToRoleAsync(user, UserRole.RestaurantOwner.ToString());

            await _auditLog.CreateAsync(new AuditLog
            {
                UserID = user.Id,
                Action = "CREATE_OWNER",
                Resource = "User",
                Metadata = $"RestaurantOwner {email} created by admin"
            });

            return user.Id;
        }

        public async Task<string> GetUserEmailAsync(int userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            return user?.Email ?? string.Empty;
        }

        public async Task<string> GetUserNameAsync(int userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            return user?.Name ?? string.Empty;
        }

        public async Task<bool> UpdateUserAsync(int id, UpdateUserDto dto)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return false;

            if (dto.Name != null) user.Name = dto.Name;
            if (dto.Phone != null) user.PhoneNumber = dto.Phone;
            if (dto.Gender != null) user.Gender = dto.Gender;
            if (dto.Address != null) user.Address = dto.Address;
            user.UpdatedAt = DateTime.UtcNow;

            return await _userRepo.UpdateAsync(user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userRepo.DeleteAsync(id);
        }
    }
}
