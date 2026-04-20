using HotByte.Modules.Identity.Application.DTOs;
using HotByte.Modules.Identity.Domain.Entities;

namespace HotByte.Modules.Identity.Application.Mappings
{
    public static class IdentityMapper
    {
        public static UserDto ToDto(AppUser user)
            => new(
                user.Id,
                user.Name,
                user.Email ?? string.Empty,
                user.PhoneNumber ?? string.Empty,
                user.Gender,
                user.Address,
                user.Role,
                user.Status,
                user.CreatedAt);

        public static IEnumerable<UserDto> ToDtoList(IEnumerable<AppUser> users)
            => users.Select(ToDto);

        public static AuditLogDto ToDto(AuditLog log)
            => new(
                log.Id,
                log.UserID,
                log.AppUser?.Name ?? string.Empty,
                log.Action,
                log.Resource,
                log.Timestamp,
                log.Metadata);

        public static IEnumerable<AuditLogDto> ToDtoList(IEnumerable<AuditLog> logs)
            => logs.Select(ToDto);
    }
}
