using StudentAttendance.Modules.Dashboard.DTOs;

namespace StudentAttendance.Modules.Dashboard.Services;

public interface IDashboardService
{
    Task<TeacherDashboardDto> GetTeacherDashboardAsync();
    Task<IEnumerable<ParentDashboardDto>> GetParentDashboardAsync(int parentId);
}
