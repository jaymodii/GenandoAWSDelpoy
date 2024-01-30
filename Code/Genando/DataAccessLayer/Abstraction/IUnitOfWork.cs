using Microsoft.EntityFrameworkCore.Storage;

namespace DataAccessLayer.Abstraction;
public interface IUnitOfWork
{
    Task SaveAsync(CancellationToken cancellationToken = default);

    Task<IDbContextTransaction> BeginTransactionAsync();

    IClinicalDetailRepository ClinicalDetailRepository { get;  }

    IClinicalQuestionRepository ClinicalQuestionRepository { get; }

    IClinicalProcessRepository ClinicalProcessRepository { get; }

    IUserRepository UserRepository { get; }
    
    ITestDetailRepository TestDetailRepository { get; }

    ITestResultRepository TestResultRepository { get; }

    IProfileRepository ProfileRepository { get; }

    IClinicalProcessStatusRepository ClinicalProcessStatusRepository { get; }

    IFAQRepository FAQRepository { get; }

    IClinicalEnhancementRepository ClinicalEnhancementRepository { get; }
    
    IRequestMoreInfoQuestionRepository RequestMoreInfoQuestionRepository { get; }

    IClinicalProcessTestRepository ClinicalProcessTestRepository { get; }
    
    IGenderRepository GenderRepository { get; }

    INotificationRepository NotificationRepository { get; }

    ISwipeActionSettingRepository SwipeActionSettingRepository { get; }
}
