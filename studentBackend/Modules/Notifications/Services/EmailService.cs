using Microsoft.Extensions.Configuration;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;
using StudentAttendance.Data.Entities;
using StudentAttendance.Modules.Notifications.Repositories;
using Task = System.Threading.Tasks.Task;

namespace StudentAttendance.Modules.Notifications.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly INotificationRepository _notificationRepository;
    private readonly TransactionalEmailsApi _apiInstance;
    private readonly string _senderEmail;
    private readonly string _senderName;

    public EmailService(IConfiguration configuration, INotificationRepository notificationRepository)
    {
        _configuration = configuration;
        _notificationRepository = notificationRepository;

        var apiKey = _configuration["BrevoSettings:ApiKey"];
        _senderEmail = _configuration["BrevoSettings:SenderEmail"] ?? "noreply@attendance-system.com";
        _senderName = _configuration["BrevoSettings:SenderName"] ?? "Student Attendance System";

        Configuration.Default.ApiKey.Clear();
        Configuration.Default.ApiKey.Add("api-key", apiKey);
        _apiInstance = new TransactionalEmailsApi();
    }

    public async Task SendAbsentNotificationAsync(int studentId, string parentEmail)
    {
        string subject = "Student Absence Notification";
        string htmlContent = $"<html><body><p>Dear Parent,</p><p>This is to inform you that your child was marked absent today.</p></body></html>";
        
        await SendEmailAsync(parentEmail, subject, htmlContent, NotificationType.Absent, studentId);
    }

    public async Task SendAttendanceWarningAsync(int studentId, string parentEmail, decimal percentage)
    {
        string subject = "Attendance Warning Notification";
        string htmlContent = $"<html><body><p>Dear Parent,</p><p>This is to inform you that your child's attendance has dropped to {percentage}%.</p></body></html>";
        
        await SendEmailAsync(parentEmail, subject, htmlContent, NotificationType.AttendanceWarning, studentId);
    }

    public async Task SendParentInvitationAsync(string parentEmail, string inviteToken, string studentName)
    {
        string subject = "Invitation to Student Attendance System";
        string inviteLink = $"http://localhost:4200/auth/parent-register?token={inviteToken}";
        string htmlContent = $"<html><body><p>Dear Parent,</p><p>You have been invited to monitor {studentName}'s attendance. Please click <a href='{inviteLink}'>here</a> to register.</p></body></html>";
        
        await SendEmailAsync(parentEmail, subject, htmlContent, NotificationType.ParentInvite, null);
    }

    public async Task SendLeaveApprovedAsync(string parentEmail, string studentName, DateTime startDate, DateTime endDate)
    {
        string subject = "Leave Request Approved";
        string htmlContent = $"<html><body><p>Dear Parent,</p><p>The leave request for {studentName} from {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd} has been approved.</p></body></html>";
        
        await SendEmailAsync(parentEmail, subject, htmlContent, NotificationType.LeaveApproved, null);
    }

    public async Task SendLeaveRejectedAsync(string parentEmail, string studentName, DateTime startDate, DateTime endDate)
    {
        string subject = "Leave Request Rejected";
        string htmlContent = $"<html><body><p>Dear Parent,</p><p>The leave request for {studentName} from {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd} has been rejected.</p></body></html>";
        
        await SendEmailAsync(parentEmail, subject, htmlContent, NotificationType.LeaveRejected, null);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string htmlContent, NotificationType type, int? studentId)
    {
        bool isSuccess = true;
        try
        {
            var sender = new SendSmtpEmailSender(_senderName, _senderEmail);
            var to = new List<SendSmtpEmailTo> { new SendSmtpEmailTo(toEmail) };
            var sendSmtpEmail = new SendSmtpEmail(sender, to, null, null, htmlContent, null, subject);
            
            // await _apiInstance.SendTransacEmailAsync(sendSmtpEmail); // Uncomment when valid API key is set
        }
        catch (Exception ex)
        {
            isSuccess = false;
            Console.WriteLine($"Error sending email: {ex.Message}");
        }
        
        var notification = new Notification
        {
            RecipientEmail = toEmail,
            Subject = subject,
            Body = htmlContent,
            Type = type,
            StudentId = studentId,
            IsSuccess = isSuccess
        };
        
        await _notificationRepository.AddNotificationAsync(notification);
    }
}
