using StudentAttendance.Data.Entities;

namespace StudentAttendance.Modules.Notifications.Repositories;

public interface INotificationRepository
{
    Task AddNotificationAsync(Notification notification);
    Task<IEnumerable<Notification>> GetNotificationsAsync();
}
