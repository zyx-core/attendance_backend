using System;
using System.Collections.Generic;
using System.Threading.Tasks; // Keeps standard async task resolution active
using Microsoft.Extensions.Configuration;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;
using StudentAttendance.Models;

namespace StudentAttendance.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly TransactionalEmailsApi _apiInstance;
        private readonly string _senderEmail;
        private readonly string _senderName;

        public EmailService(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;

            var apiKey = _configuration["BrevoSettings:ApiKey"];
            _senderEmail = _configuration["BrevoSettings:SenderEmail"] ?? "noreply@attendance-system.com";
            _senderName = _configuration["BrevoSettings:SenderName"] ?? "Student Attendance System";

            Configuration.Default.ApiKey.Clear();
            Configuration.Default.ApiKey.Add("api-key", apiKey);
            _apiInstance = new TransactionalEmailsApi();
        }

        // FIXED: Explicitly specified System.Threading.Tasks.Task to clear namespace clash
        public async System.Threading.Tasks.Task SendAbsentNotificationAsync(int studentId, string parentEmail, string studentName)
        {
            string subject = "Child Absence Notification";
            string htmlContent = $"<html><body><h3>Dear Parent,</h3><p>This is an automated message to inform you that <strong>{studentName}</strong> was marked <strong>Absent</strong> from classes today.</p></body></html>";
            
            await SendEmailInstanceAsync(parentEmail, subject, htmlContent, NotificationType.Absent, studentId);
        }

        // FIXED: Explicitly specified System.Threading.Tasks.Task to clear namespace clash
        public async System.Threading.Tasks.Task SendAttendanceWarningAsync(int studentId, string parentEmail, string studentName, decimal percentage)
        {
            string subject = "CRITICAL: Attendance Dropped Below Threshold";
            string htmlContent = $"<html><body><h3>Attendance Warning Alert</h3><p>Dear Parent,</p><p>Please be advised that the cumulative attendance record for <strong>{studentName}</strong> has dropped down to critical levels at <strong>{percentage}%</strong>.</p></body></html>";
            
            await SendEmailInstanceAsync(parentEmail, subject, htmlContent, NotificationType.AttendanceWarning, studentId);
        }

        // FIXED: Explicitly specified System.Threading.Tasks.Task to clear namespace clash
        public async System.Threading.Tasks.Task SendParentInvitationAsync(string parentEmail, string inviteToken, string studentName)
        {
            string subject = "Invitation to Student Portal Workspace";
            string registrationUrl = $"http://localhost:4200/register-parent?token={inviteToken}";
            string htmlContent = $"<html><body><h3>Portal Invitation</h3><p>You have been invited to monitor <strong>{studentName}'s</strong> attendance dashboards. Click the link below to set up your account profile:</p><a href='{registrationUrl}'>Complete Registration</a></body></html>";
            
            await SendEmailInstanceAsync(parentEmail, subject, htmlContent, NotificationType.ParentInvite, null);
        }

        // FIXED: Explicitly specified System.Threading.Tasks.Task to clear namespace clash
        private async System.Threading.Tasks.Task SendEmailInstanceAsync(string toEmail, string subject, string htmlContent, NotificationType type, int? studentId)
        {
            bool isSuccess = true;
            try
            {
                var sender = new SendSmtpEmailSender(_senderName, _senderEmail);
                var receiver = new SendSmtpEmailTo(toEmail);
                var smtpEmail = new SendSmtpEmail(
                    sender, 
                    new List<SendSmtpEmailTo> { receiver }, 
                    htmlContent: htmlContent, 
                    subject: subject
                );

                await _apiInstance.SendTransacEmailAsync(smtpEmail);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Console.WriteLine($"[Brevo Notification Error]: {ex.Message}");
            }

            var log = new Notification
            {
                StudentId = studentId,
                RecipientEmail = toEmail,
                Type = type,
                Subject = subject,
                Body = htmlContent,
                IsSuccess = isSuccess
            };

            await _context.Notifications.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }
}