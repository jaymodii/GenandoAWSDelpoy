using AutoMapper;
using BusinessAccessLayer.Abstraction;
using Common.Constants;
using Common.Utils;
using DataAccessLayer.Abstraction;
using Entities.DataModels;
using Entities.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessAccessLayer.Implementation
{
    public class NotificationService : GenericService<Notification>, INotificationService
    {
        #region Properties

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuthenticationService _authenticationService;

        #endregion Properties

        #region Constructors

        public NotificationService(IUnitOfWork unitOfWork, IMapper mapper, IAuthenticationService authenticationService
            ) : base(unitOfWork.NotificationRepository, unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _authenticationService = authenticationService;
        }

        public async Task<int> GetNotificationCountAsync(long userId)
        {
            IEnumerable<Notification> notifications = await _unitOfWork.NotificationRepository.GetAllAsync(notifications => notifications.SendTo == userId && !notifications.HasRead && !notifications.IsTempDeleted);

            return notifications.Count();
        }

        public async Task<IEnumerable<NotificationResultDTO>> GetNotificationResultsAsync(long userId)
        {
            IEnumerable<Notification> notifications = await _unitOfWork.NotificationRepository.GetAllAsync(notifications => notifications.SendTo == userId);

            if (notifications != null)
            {
                IEnumerable<NotificationResultDTO> notificationResults = _mapper.Map<IEnumerable<NotificationResultDTO>>(notifications);

                foreach (NotificationResultDTO? result in notificationResults)
                {
                    TimeSpan timeDifference = DateTimeOffset.Now - result.CreatedOn;
                    result.Time = FormatTimeDifference(timeDifference);
                }

                notificationResults = notificationResults.OrderByDescending(result => result.CreatedOn);

                return notificationResults;
            }

            return Enumerable.Empty<NotificationResultDTO>();
        }

        public async Task ReadNotificationsAsync(long userId, long? notificationId)
        {
            if (notificationId != null)
            {
                Notification? notification = await _unitOfWork.NotificationRepository.GetFirstOrDefaultAsync(
               notification => notification.Id == notificationId && notification.SendTo == userId && !notification.HasRead);

                if (notification != null)
                {
                    notification.HasRead = ModelConstants.TrueConstant;
                    await UpdateAsync(notification);
                }
            }
            else
            {
                IEnumerable<Notification> notifications = await _unitOfWork.NotificationRepository.GetAllAsync(notifications => notifications.SendTo == userId && !notifications.HasRead);
                foreach (Notification notification in notifications)
                {
                    notification.HasRead = ModelConstants.TrueConstant;
                }
                await UpdateRangeAsync(notifications);
            }
        }

        public async Task DeleteNotificationsAsync(long userId, long? notificationId)
        {
            if(notificationId != null)
            {
                Notification? notification = await _unitOfWork.NotificationRepository.GetFirstOrDefaultAsync(
               notification => notification.Id == notificationId && notification.SendTo == userId);
                if(notification != null)
                {
                    notification.IsTempDeleted = ModelConstants.TrueConstant;
                    notification.HasRead = ModelConstants.TrueConstant;
                    await UpdateAsync(notification);
                }
            }
            else
            {
                IEnumerable<Notification> notifications = await _unitOfWork.NotificationRepository.GetAllAsync(notifications => notifications.SendTo == userId);
                await RemoveRangeAsync(notifications);
            }
        }

        #endregion

        #region helper methods

        private string FormatTimeDifference(TimeSpan timeDifference)
        {
            if (timeDifference.TotalDays >= 365)
            {
                int years = (int)(timeDifference.TotalDays / 365);
                return $"{years} year{(years >= 2 ? "s" : "")} ago";
            }
            else if (timeDifference.TotalDays >= 1)
            {
                int days = (int)timeDifference.TotalDays;
                return $"{days} day{(days >= 2 ? "s" : "")} ago";
            }
            else if (timeDifference.TotalHours >= 1)
            {
                int hours = (int)timeDifference.TotalHours;
                return $"{hours} hour{(hours >= 2 ? "s" : "")} ago";
            }
            else if (timeDifference.TotalMinutes >= 1)
            {
                int minutes = (int)timeDifference.TotalMinutes;
                return $"{minutes} minute{(minutes >= 2 ? "s" : "")} ago";
            }
            else
            {
                return "just now";
            }
        }
        #endregion
    }
}
