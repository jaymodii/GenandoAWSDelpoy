using DataAccessLayer.Abstraction;
using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace DataAccessLayer.Implementation;

public class UnitOfWork : IUnitOfWork
{
    #region Properties

    private readonly AppDbContext _dbContext;
    private IClinicalDetailRepository _clinicalDetailRepository;
    private IClinicalQuestionRepository _clinicalQuestionRepository;
    private IClinicalProcessRepository _clinicalProcessRepository;
    private IClinicalProcessStatusRepository _clinicalProcessStatusRepository;
    private IUserRepository _userRepository;
    private ITestDetailRepository _testDetailRepository;
    private ITestResultRepository _testResultRepository;
    private IProfileRepository _profileRepository;
    private IFAQRepository _faqRepository;
    private IClinicalEnhancementRepository _clinicalEnhancementRepository;
    private IRequestMoreInfoQuestionRepository _requestMoreInfoQuestionRepository;
    private IGenderRepository _genderRepository;
    private IClinicalProcessTestRepository _clinicalProcessTestRepository;
    private INotificationRepository _notificationRepository;
    private ISwipeActionSettingRepository _swipeActionSettingRepository;

    #endregion Properties

    #region Constructor

    public UnitOfWork(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #endregion Constructor

    #region Interface methods

    public async Task SaveAsync(CancellationToken cancellationToken = default)
        => await _dbContext.SaveChangesAsync(cancellationToken);

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        if (_dbContext.Database.CurrentTransaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        return await _dbContext.Database.BeginTransactionAsync();
    }

    public IGenericRepository<T> GetRepository<T>() where T : class
    {
        return new GenericRepository<T>(_dbContext);
    }

    public IClinicalDetailRepository ClinicalDetailRepository
    {
        get
        {
            return _clinicalDetailRepository ??= new ClinicalDetailRepository(_dbContext);
        }
    }

    public IClinicalQuestionRepository ClinicalQuestionRepository
    {
        get
        {
            return _clinicalQuestionRepository ??= new ClinicalQuestionRepository(_dbContext);
        }
    }

    public IClinicalProcessRepository ClinicalProcessRepository
    {
        get
        {
            return _clinicalProcessRepository ??= new ClinicalProcessRepository(_dbContext);
        }
    }

    public IClinicalProcessStatusRepository ClinicalProcessStatusRepository
    {
        get
        {
            return _clinicalProcessStatusRepository ??= new ClinicalProcessStatusRepository(_dbContext);
        }
    }

    public IProfileRepository ProfileRepository
    {
        get
        {
            return _profileRepository ??= new ProfileRepository(_dbContext);
        }
    }

    public IFAQRepository FAQRepository
    {
        get
        {
            return _faqRepository ??= new FAQRepository(_dbContext);
        }
    }

    public IUserRepository UserRepository
    {
        get
        {
            return _userRepository ??= new UserRepository(_dbContext);
        }
    }

    public ITestDetailRepository TestDetailRepository
    {
        get
        {
            return _testDetailRepository ??= new TestDetailRepository(_dbContext);
        }
    }

    public ITestResultRepository TestResultRepository
    {
        get
        {
            return _testResultRepository ??= new TestResultRepository(_dbContext);
        }
    }

    public IClinicalEnhancementRepository ClinicalEnhancementRepository
    {
        get
        {
            return _clinicalEnhancementRepository ??= new ClinicalEnhancementRepository(_dbContext);
        }
    }
    
    public IRequestMoreInfoQuestionRepository RequestMoreInfoQuestionRepository
    {
        get
        {
            return _requestMoreInfoQuestionRepository ??= new RequestMoreInfoQuestionRepository(_dbContext);
        }
    }

    public IGenderRepository GenderRepository
    {
        get
        {
            return _genderRepository ??= new GenderRepository(_dbContext);
        }
    }

    public IClinicalProcessTestRepository ClinicalProcessTestRepository
    {
        get
        {
            return _clinicalProcessTestRepository ??= new ClinicalProcessTestRepository(_dbContext);
        }
    }
    
    public INotificationRepository NotificationRepository
    {
        get
        {
            return _notificationRepository ??= new NotificationRepository(_dbContext);
        }
    }

    public ISwipeActionSettingRepository SwipeActionSettingRepository
    {
        get
        {
            return _swipeActionSettingRepository ??= new SwipeActionSettingRepository(_dbContext);
        }
    }
    #endregion Interface methods
}