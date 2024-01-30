using DataAccessLayer.Abstraction;
using DataAccessLayer.Data;
using Entities.DataModels;

namespace DataAccessLayer.Implementation
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        #region Properties

        #endregion

        #region Constructors

        public NotificationRepository(AppDbContext dbContext)
            : base(dbContext)
        {
        }

        #endregion

    }
}
