using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HotByte.Modules.Identity.Application.DTOs;
using HotByte.Modules.Identity.Application.Interfaces;
using HotByte.Modules.Identity.Domain.Entities;

namespace HotByte.Modules.Identity.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _config;
        private readonly IAuditLogRepository _auditLog;
        private readonly IIdentityEmailService _emailService;

        public AuthService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IConfiguration config,
            IAuditLogRepository auditLog,
            IIdentityEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _auditLog = auditLog;
            _emailService = emailService;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return null;

            var result = await _signInManager.CheckPasswordSignInAsync(
                user, dto.Password, lockoutOnFailure: true);
            if (!result.Succeeded) return null;

            await _auditLog.CreateAsync(new AuditLog
            {
                UserID = user.Id,
                Action = "LOGIN",
                Resource = "System",
                Metadata = "Success"
            });

            var accessToken = GenerateJwtToken(user);
            var expiresAt = DateTime.UtcNow.AddMinutes(60);

            return new AuthResponseDto(
                accessToken,
                expiresAt,
                user.Id,
                user.Email ?? string.Empty,
                user.Name,
                user.Role.ToString());
        }

        public async Task<bool> LogoutAsync(int userId)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;

            await _auditLog.CreateAsync(new AuditLog
            {
                UserID = userId,
                Action = "LOGOUT",
                Resource = "System",
                Metadata = "Success"
            });

            return true;
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(token);
            var frontendUrl = _config["App:FrontendUrl"] ?? "http://localhost:3000";
            var resetLink = $"{frontendUrl}/reset-password?userId={user.Id}&token={encodedToken}";

            var body = $@"
                <div style='font-family:Arial,sans-serif;max-width:480px;margin:auto;padding:24px;border:1px solid #e5e7eb;border-radius:12px'>
                  <h2 style='color:#2563eb'>HotByte — Reset Your Password</h2>
                  <p>Hi {user.Name},</p>
                  <p>We received a request to reset your password. Click the button below to choose a new password.</p>
                  <p style='text-align:center;margin:32px 0'>
                    <a href='{resetLink}' style='background:#2563eb;color:#fff;padding:12px 28px;border-radius:8px;text-decoration:none;font-weight:bold'>Reset Password</a>
                  </p>
                  <p style='color:#6b7280;font-size:13px'>This link expires in 1 hour. If you did not request a password reset, you can safely ignore this email.</p>
                </div>";

            await _emailService.SendAsync(user.Email!, "Reset your HotByte password", body);

            await _auditLog.CreateAsync(new AuditLog
            {
                UserID = user.Id,
                Action = "FORGOT_PASSWORD",
                Resource = "System",
                Metadata = "Reset email sent"
            });

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == dto.UserId);
            if (user == null) return false;

            var result = await _userManager.ResetPasswordAsync(
                user, dto.Token, dto.NewPassword);

            if (result.Succeeded)
            {
                await _auditLog.CreateAsync(new AuditLog
                {
                    UserID = user.Id,
                    Action = "RESET_PASSWORD",
                    Resource = "System",
                    Metadata = "Success"
                });
            }

            return result.Succeeded;
        }

        private string GenerateJwtToken(AppUser user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("name", user.Name),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("role", user.Role.ToString()),
                new Claim("userId", user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: new SigningCredentials(
                    key, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
