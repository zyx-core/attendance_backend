using Microsoft.EntityFrameworkCore;
using StudentAttendance.Data;
using StudentAttendance.Data.Entities;

namespace StudentAttendance.Modules.Notifications.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;

    public NotificationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddNotificationAsync(Notification notification)
    {
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Notification>> GetNotificationsAsync()
    {
        return await _context.Notifications.ToListAsync();
    }
}
