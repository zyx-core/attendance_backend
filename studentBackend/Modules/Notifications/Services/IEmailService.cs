namespace StudentAttendance.Modules.Notifications.Services;

public interface IEmailService
{
    Task SendAbsentNotificationAsync(int studentId, string parentEmail);
    Task SendAttendanceWarningAsync(int studentId, string parentEmail, decimal percentage);
    Task SendParentInvitationAsync(string parentEmail, string inviteToken, string studentName);
    Task SendLeaveApprovedAsync(string parentEmail, string studentName, DateTime startDate, DateTime endDate);
    Task SendLeaveRejectedAsync(string parentEmail, string studentName, DateTime startDate, DateTime endDate);
}
