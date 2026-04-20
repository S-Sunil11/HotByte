using System.ComponentModel.DataAnnotations;

namespace HotByte.Modules.Identity.Application.DTOs
{
    public record LoginDto(
        [Required][EmailAddress] string Email,
        [Required] string Password);

    public record AuthResponseDto(
        string AccessToken,
        DateTime ExpiresAt,
        int UserId,
        string Email,
        string Name,
        string Role);

    public record ForgotPasswordDto([Required][EmailAddress] string Email);

    public record ResetPasswordDto(
        [Required] int UserId,
        [Required] string Token,
        [Required][MinLength(8)] string NewPassword);
}
