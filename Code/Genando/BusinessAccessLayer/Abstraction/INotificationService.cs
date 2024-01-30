using Entities.DataModels;
using Entities.DTOs.Request;

namespace BusinessAccessLayer.Abstraction
{
    public interface INotificationService : IGenericService<Notification>
    {
        Task<int> GetNotificationCountAsync(long userId);

        Task<IEnumerable<NotificationResultDTO>> GetNotificationResultsAsync(long userId);

        Task ReadNotificationsAsync(long userId, long? notificationId);

        Task DeleteNotificationsAsync(long userId, long? notificationId);

    }
}
