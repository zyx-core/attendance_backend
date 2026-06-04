using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentAttendance.DTOs;
using StudentAttendance.Models;

namespace StudentAttendance.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly AppDbContext _context;

        public AnalyticsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            // Note: When Member 2 connects the Students table, replace this with actual DB count
            int totalStudents = 10; // Temporary placeholder until Students entity is merged

            int pendingLeaves = await _context.LeaveRequests
                .CountAsync(l => l.Status == LeaveStatus.Pending);

            // Calculate mock average or compute from data entries
            decimal attendanceAvg = 84.5m; 
            int riskCount = 2;

            return new DashboardStatsDto
            {
                TotalStudents = totalStudents,
                AttendanceAverage = attendanceAvg,
                PendingLeaves = pendingLeaves,
                RiskStudentsCount = riskCount
            };
        }

        public async Task<RiskAnalysisDto> GetRiskAnalysisAsync()
        {
            // Simulating analytical breakdown for dashboard charts
            var mockStudents = new List<AttendanceSummaryDto>
            {
                new() { StudentId = 101, StudentName = "Rahul Sharma", TotalClasses = 40, PresentCount = 38, AbsentCount = 2, Percentage = 95.0m, RiskLevel = "Low" },
                new() { StudentId = 102, StudentName = "Sneha Patel", TotalClasses = 40, PresentCount = 32, AbsentCount = 8, Percentage = 80.0m, RiskLevel = "Medium" },
                new() { StudentId = 103, StudentName = "Amir Khan", TotalClasses = 40, PresentCount = 18, AbsentCount = 22, Percentage = 45.0m, RiskLevel = "High" }
            };

            return new RiskAnalysisDto
            {
                LowRiskCount = mockStudents.Count(s => s.RiskLevel == "Low"),
                MediumRiskCount = mockStudents.Count(s => s.RiskLevel == "Medium"),
                HighRiskCount = mockStudents.Count(s => s.RiskLevel == "High"),
                AtRiskStudents = mockStudents.Where(s => s.RiskLevel == "High" || s.RiskLevel == "Medium").ToList()
            };
        }
    }
}