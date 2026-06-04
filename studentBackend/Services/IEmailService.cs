using System.Threading.Tasks;

namespace StudentAttendance.Services
{
    public interface IEmailService
    {
        Task SendAbsentNotificationAsync(int studentId, string parentEmail, string studentName);
        Task SendAttendanceWarningAsync(int studentId, string parentEmail, string studentName, decimal percentage);
        Task SendParentInvitationAsync(string parentEmail, string inviteToken, string studentName);
    }
}