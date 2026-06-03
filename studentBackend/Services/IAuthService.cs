using System.Threading.Tasks;
using StudentAttendance.DTOs;

namespace StudentAttendance.Services
{
    public interface IAuthService
    {
        Task<bool> RegisterTeacherAsync(RegisterTeacherDto dto);
        Task<string?> RegisterParentAsync(AcceptInvitationDto dto);
        Task<string?> LoginAsync(LoginDto dto);
    }
}
