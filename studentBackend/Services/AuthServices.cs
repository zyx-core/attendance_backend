using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudentAttendance.DTOs;
using StudentAttendance.Models;
namespace StudentAttendance.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RegisterTeacherAsync(RegisterTeacherDto dto)
        {
            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return false;

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "Teacher",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string?> RegisterParentAsync(AcceptInvitationDto dto)
        {
            // Find the invitation tracking token
            var invite = await _context.ParentInvitations
                .FirstOrDefaultAsync(i => i.InviteToken == dto.Token);

            if (invite == null || invite.IsUsed || invite.ExpiresAt < DateTime.UtcNow)
                return "Invalid or expired invitation token.";

            if (!invite.ParentEmail.Equals(dto.Email, StringComparison.OrdinalIgnoreCase))
                return "The email provided does not match the invitation record.";

            // 1. Create Parent User Account
            var user = new User
            {
                FullName = "Parent Account", // Can update later on user profile edit
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "Parent",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync(); // Generates the User Id

            // 2. Link Parent with Student mapping row
            var parentStudent = new ParentStudent
            {
                ParentId = user.Id,
                StudentId = invite.StudentId,
                Relationship = "Parent"
            };
            _context.ParentStudent.Add(parentStudent);

            // 3. Close out the invitation
            invite.IsUsed = true;
            
            await _context.SaveChangesAsync();
            return null; // Return null if there are no errors
        }

        public async Task<string?> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !user.IsActive)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "this_is_a_fallback_secret_key_that_must_be_long_enough_for_hmac_sha256";
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<(bool Success, string Message, string? ResetToken)> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            // Always return success to avoid email enumeration attacks,
            // but only create a token if the user exists.
            if (user == null || !user.IsActive)
                return (true, "If an account with that email exists, a reset token has been sent.", null);

            // Invalidate any existing unused tokens for this email
            var existingTokens = await _context.PasswordResetTokens
                .Where(t => t.Email == dto.Email && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();
            foreach (var t in existingTokens)
                t.IsUsed = true;

            // Generate a secure random token (URL-safe base64)
            var rawToken = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32))
                .Replace("+", "-").Replace("/", "_").Replace("=", "");

            var resetToken = new PasswordResetToken
            {
                Email = dto.Email,
                Token = rawToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.PasswordResetTokens.Add(resetToken);
            await _context.SaveChangesAsync();

            // In a real system you would email the token. For development,
            // we return it in the response so it can be used directly on the reset page.
            return (true, "Password reset token generated successfully.", rawToken);
        }

        public async Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Token) || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.NewPassword))
                return (false, "All fields are required.");

            if (dto.NewPassword.Length < 6)
                return (false, "Password must be at least 6 characters.");

            var resetToken = await _context.PasswordResetTokens
                .FirstOrDefaultAsync(t => t.Token == dto.Token && t.Email == dto.Email);

            if (resetToken == null || resetToken.IsUsed || resetToken.ExpiresAt < DateTime.UtcNow)
                return (false, "Invalid or expired reset token.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !user.IsActive)
                return (false, "User account not found.");

            // Update the password hash
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            // Mark the token as used so it can't be replayed
            resetToken.IsUsed = true;

            await _context.SaveChangesAsync();
            return (true, "Password has been reset successfully. You can now log in.");
        }
    }
}