using System;
using System.IdentityModel.Tokens.Jwt;
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
    }
}