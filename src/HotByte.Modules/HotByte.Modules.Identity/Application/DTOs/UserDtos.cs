using System.ComponentModel.DataAnnotations;
using static HotByte.SharedKernel.Enums.Enum;

namespace HotByte.Modules.Identity.Application.DTOs
{
    // Customer-only self-registration (Role is forced to Customer server-side)
    public record RegisterCustomerDto(
        [Required][MaxLength(150)] string Name,
        [Required][EmailAddress] string Email,
        [Required][MinLength(8)] string Password,
        [MaxLength(20)] string Phone,
        string? Gender,
        string? Address);

    public record UserDto(
        int Id,
        string Name,
        string Email,
        string Phone,
        string? Gender,
        string? Address,
        UserRole Role,
        UserStatus Status,
        DateTime CreatedAt);

    public record UpdateUserDto(
        [MaxLength(150)] string? Name,
        [MaxLength(20)] string? Phone,
        string? Gender,
        string? Address);

    public record AuditLogDto(
        int Id,
        int UserID,
        string UserName,
        string Action,
        string Resource,
        DateTime Timestamp,
        string? Metadata);
}
