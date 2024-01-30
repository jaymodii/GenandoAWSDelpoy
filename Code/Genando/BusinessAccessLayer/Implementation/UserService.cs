using AutoMapper;
using BusinessAccessLayer.Abstraction;
using Common.Constants;
using Common.Enums;
using Common.Exceptions;
using Common.Hubs;
using Common.Utils;
using Common.Utils.Model;
using DataAccessLayer.Abstraction;
using DataAccessLayer.Helpers;
using Entities.DataModels;
using Entities.DTOs.Request;
using Entities.DTOs.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;
using System.Transactions;

namespace BusinessAccessLayer.Implementation;

public class UserService
    : GenericService<User>, IUserService
{
    #region Properties

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMailService _mailService;
    private readonly IClinicalProcessService _clinicalProcessService;
    private JwtManageService _jwtManageService;
    private readonly IClinicalDetailService _clinicalDetailService;
    private readonly IHubContext<BroadcastHub> _hubContext;

    #endregion Properties

    #region Constructors

    public UserService(IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IClinicalDetailService clinicalDetailService,
        IMailService mailService,
        IClinicalProcessService clinicalProcessService,
        IConfiguration config,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper,
        IHubContext<BroadcastHub> hubContext)
        : base(userRepository, unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _clinicalDetailService = clinicalDetailService;
        _mailService = mailService;
        _clinicalProcessService = clinicalProcessService;
        _jwtManageService=new JwtManageService(config,httpContextAccessor);
        _mapper = mapper;
        _hubContext = hubContext;
    }

    #endregion Constructors

    #region Interface Methods

    public async Task<long> RegisterAsync(UserDetailsFormRequestDto dto,
        LoggedUser loggedUser,
        CancellationToken cancellationToken = default)
    {
        User user = GetUserModel(dto);
        bool isPatient = dto.IsPatient;

        if (isPatient)
        {
            await AdditionalNewPatientDetails(loggedUser, user, cancellationToken);
        }

        string password = SetSaltedPassword(user);

        user.DoctorId = loggedUser.UserId;

        await AddAsync(user, cancellationToken);

        await _unitOfWork.SaveAsync(cancellationToken);

        await SendAccountCreateNotification(user, password, cancellationToken);

        await CreateClinicalQuestion(user);

        if (isPatient)
            await AddPatientToClinicalProcess(user);

        await AddSwipeActionSetting(user);

        await _unitOfWork.SaveAsync(cancellationToken);

        return user.Id;
    }

    public async Task<bool> IsDuplicateEmail(string email,
        long? userId,
        CancellationToken cancellationToken = default)
        => userId is null ? await AnyAsync(EmailFilter(email), cancellationToken)
            : await AnyAsync(EmailFilter(email, userId), cancellationToken);

    public async Task<PageInformationDto<UserListingResponseDto>> SearchAsync(PatientPageRequestDto dto,
        long doctorId,
        CancellationToken cancellationToken = default)
    {
        PageFilterCriteria<User> criteria = CreatePatientListingCriteria(dto, doctorId);

        (long count, IEnumerable<User> data) = await GetAllAsync(criteria, cancellationToken);

        PageInformationDto<UserListingResponseDto> patientRecords = GetPageInformation(count,
            data,
            criteria.PageRequest);

        return patientRecords;
    }

    public async Task<UserDetailsFormResponseDto> LoadUserProfile(long id,
        CancellationToken cancellationToken = default)
    {
        User model = await GetUserByIdAsync(id, null, cancellationToken);

        return _mapper.Map<UserDetailsFormResponseDto>(model);
    }

    public async Task<UserListingResponseDto?> LoadDoctorLabUser(long doctorId,
        CancellationToken cancellationToken = default)
    {
        User? model = await GetFirstOrDefaultAsync(DoctorLabUserFilter(doctorId), cancellationToken);

        return model is null ? null : _mapper.Map<UserListingResponseDto>(model);
    }

    public async Task EditUserProfile(long id,
        UserDetailsFormRequestDto dto, LoggedUser loggedUser,
        CancellationToken cancellationToken = default)
    {
        User? user = await _unitOfWork.UserRepository.GetAsync(
          user => user.Id == id,
          new Expression<Func<User, object>>[] { x => x.DoctorUsers },
          new string[] { "DoctorUsers" }
        );
        _mapper.Map(dto, user);

        string gender = user.Gender == 1 ? "Male" : "Female";

        await UpdateAsync(user, cancellationToken);

        Notification notification = new Notification()
        {
            SentBy = loggedUser.UserId,
            SendTo = user.Id,
            NotificationMessage = $"Dr. {loggedUser.Name} has updated your profile. Please Check your email for further details."
        };

        await _unitOfWork.NotificationRepository.AddAsync(notification);

        await _hubContext.Clients.All.SendAsync("BroadcastMessage");

        await _unitOfWork.SaveAsync(cancellationToken);

        MailDto mailDto = new()
        {
            ToEmail = user.Email,
            Subject = MailConstants.UpdatedPatientDetailsMailSubject,
            Body = MailBodyUtil.SendPatientDetailsUpdatedEmail($"{user.FirstName} {user.LastName}", user.Email,
            (DateTimeOffset)user.DOB, user.PhoneNumber, gender, user.Address, $"{user.DoctorUsers.FirstName} {user.DoctorUsers.LastName}")
        };
        await _mailService.SendMailAsync(mailDto);
    }

    public async Task DeleteUserAsync(long id,
        CancellationToken cancellationToken = default)
    {
        User model = await GetUserByIdAsync(id, null, cancellationToken: cancellationToken);

        await HandleUserDeletion(model, cancellationToken);

        model.Status = 3;
        await UpdateAsync(model, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);
    }

    public async Task<ContactDoctorDto> GetDoctorDetails()
    {
        long id = _jwtManageService.GetLoggedUser().UserId;
        User model = await GetUserByIdAsync(id, true);
        long doctorId = model.DoctorId!.Value;
        model = await GetUserByIdAsync(doctorId, null);

        return _mapper.Map<ContactDoctorDto>(model);
    }
    #endregion Interface Methods

    #region Helper methods

    private async Task AdditionalNewPatientDetails(LoggedUser loggedUser, 
        User user, 
        CancellationToken cancellationToken)
    {
        user.LabId = await GetLabIdForDoctor(loggedUser.UserId, cancellationToken);

        if (user.LabId is null)
            throw new ModelValidationException(MessageConstants.LabUserNotFound);

        user.ConsultationStatus = PatientConsultationStatusType.New;
    }

    private static string SetSaltedPassword(User user)
    {
        string password = PasswordUtil.GeneratePassword();
        user.Password = PasswordUtil.HashPassword(password);

        return password;
    }

    private async Task AddSwipeActionSetting(User user)
    {
        SwipeActionSetting swipeActionSetting = new SwipeActionSetting
        {
            UserId = user.Id,
        };

        await _unitOfWork.SwipeActionSettingRepository.AddAsync(swipeActionSetting);
    }

    private User GetUserModel(UserDetailsFormRequestDto dto)
        => _mapper.Map<User>(dto);

    private IEnumerable<UserListingResponseDto> GetPatientListingDto(IEnumerable<User> models)
        => _mapper.Map<IEnumerable<UserListingResponseDto>>(models);

    private async Task CreateClinicalQuestion(User user)
    {
        if (user.Role != (byte)UserRoleType.Patient) return;

        await _clinicalDetailService.AddClinicalQuestions(user.Id);
    }

    private async Task AddPatientToClinicalProcess(User user)
    {
        ClinicalProcess clinicalProcess = new()
        {
            PatientId = user.Id,
            Status = SystemConstants.InitialStatus,
            NextStep = SystemConstants.InitialStatus
        };

        await _clinicalProcessService.AddAsync(clinicalProcess);
    }

    private PageFilterCriteria<User> CreatePatientListingCriteria(PatientPageRequestDto dto,
        long doctorId)
    {
        PageFilterCriteria<User> criteria = new();

        criteria.PageRequest = dto.PageRequest!;

        string? searchKey = criteria.PageRequest.SearchKey;

        if (!string.IsNullOrEmpty(searchKey)
            || dto.Gender is not null)
        {
            if (!string.IsNullOrEmpty(searchKey)
                && dto.Gender is not null)
            {
                criteria.Filter = SearchWithGenderFilter(searchKey, dto.Gender.Value);
            }
            else if (!string.IsNullOrEmpty(searchKey))
            {
                criteria.Filter = SearchFilter(searchKey);
            }
            else
            {
                criteria.Filter = GenderFilter(dto.Gender!.Value);
            }
        }

        criteria.StatusFilter = dto.Status is not null
            ? ConsultationStatusFilter(dto.Status.Value, doctorId)
            : StatusFilter(doctorId);

        criteria.Select = PatientDashboardSelect;

        criteria.OrderByDescending = OrderByDescCreatedOn;

        return criteria;
    }

    private PageInformationDto<UserListingResponseDto> GetPageInformation(long count,
        IEnumerable<User> data,
        PageRequestDto pageRequest)
        => new(count,
            GetPatientListingDto(data),
            PageCountHelper.GetTotalPage(count, pageRequest.PageSize),
            pageRequest.PageNumber,
            pageRequest.PageSize);

    private async Task<User> GetUserByIdAsync(long id,
        bool? isPatient,
        CancellationToken cancellationToken = default)
    {
        User? model;
        if (isPatient.HasValue)
        {
            model = await GetFirstOrDefaultAsync(isPatient.Value ? GetPatientByIdFilter(id) : GetLabByIdFilter(id), cancellationToken);
        }
        else
        {
            model = await GetFirstOrDefaultAsync(GetUserByIdFilter(id), cancellationToken);
        }
        return model is null
            ? throw new ResourceNotFoundException(MessageConstants.USER_RESOURCE_NOT_FOUND.Replace("#", id.ToString()))
            : model;
    }

    private async Task SendAccountCreateNotification(User user,
        string password,
        CancellationToken cancellationToken = default)
    {
        MailDto dto = new()
        {
            ToEmail = user.Email,
            Subject = MessageConstants.AccountCreated,
            Body = MailBodyUtil.CreateNewAccountNotification($"{user.FirstName} {user.LastName}",
                                user.Email,
                                password)
        };

        await _mailService.SendMailAsync(dto, cancellationToken);
    }

    private async Task HandleUserDeletion(User model,
        CancellationToken cancellationToken)
    {
        switch ((UserRoleType)model.Role)
        {
            case UserRoleType.Doctor:
                throw new ForbiddenException(MessageConstants.FORBID_USER_DELETE);

            case UserRoleType.Patient:
                if (model.ConsultationStatus == PatientConsultationStatusType.OnGoing)
                    throw new ModelValidationException(errorList: MessageConstants.DeleteUserError);
                break;

            case UserRoleType.Lab:
                bool isLabDeletable = await GetLabDeleteStatus(model.Id, cancellationToken);
                if (!isLabDeletable)
                    throw new ModelValidationException(errorList: MessageConstants.DeleteLabError);
                break;
        }
    }

    private async Task<bool> GetLabDeleteStatus(long id,
        CancellationToken cancellationToken)
        => !(await AnyAsync(IsLabDeletableFilter(id), cancellationToken));

    public async Task<string> GetAvatar(long id)
    {
        User? user = await _unitOfWork.ProfileRepository.GetFirstOrDefaultAsync(user => user.Id == id);

        string avatar = "";

        if (user.Avatar != null)
        {
            byte[]? byteData = user.Avatar;

            string imreBase64Data = Convert.ToBase64String(byteData);
            string imgDataURL = string.Format("data:image/png;base64,{0}", imreBase64Data);

            avatar = imgDataURL;
        }

        return avatar;
    }

    private async Task<long?> GetLabIdForDoctor(long doctorId,
        CancellationToken cancellationToken = default)
    {
        User? model = await GetFirstOrDefaultAsync(DoctorLabUserFilter(doctorId), cancellationToken);

        return model?.Id;
    }

    #endregion Helper methods

    #region Helper Filters

    private static Expression<Func<User, bool>> EmailFilter(string email,
        long? userId = null)
        => userId is null ? user => user.Email == email
                            : user => user.Email == email && user.Id != userId;

    private static Expression<Func<User, bool>> SearchWithGenderFilter(string searchKey,
        GenderType gender)
        => user => (user.FirstName.Contains(searchKey) || user.LastName.Contains(searchKey)
                    || user.PhoneNumber.Contains(searchKey) || user.Email.Contains(searchKey))
                    && user.Gender == (byte)gender;

    private static Expression<Func<User, bool>> SearchFilter(string searchKey)
    => user => user.FirstName.Contains(searchKey) || user.LastName.Contains(searchKey)
                || user.PhoneNumber.Contains(searchKey) || user.Email.Contains(searchKey);

    private static Expression<Func<User, bool>> GenderFilter(GenderType gender)
        => user => user.Gender == (byte)gender;

    private static Expression<Func<User, bool>> StatusFilter(long doctorId,
        byte role = (byte)UserRoleType.Patient)
            => user => user.Status == EntityStatusConstants.ACTIVE && user.Role == role && user.DoctorId == doctorId;

    private static Expression<Func<User, bool>> ConsultationStatusFilter(PatientConsultationStatusType status,
        long doctorId)
        => user => user.Status == EntityStatusConstants.ACTIVE && user.Role == (byte)UserRoleType.Patient
                    && user.ConsultationStatus == status && user.DoctorId == doctorId;

    private static Expression<Func<User, User>> PatientDashboardSelect
        => user => new()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            DOB = user.DOB,
            Gender = user.Gender,
            ConsultationStatus = user.ConsultationStatus,
            Avatar = user.Avatar
        };

    private static Expression<Func<User, bool>> GetPatientByIdFilter(long id)
        => user => user.Id == id && user.Status == EntityStatusConstants.ACTIVE && user.Role == (byte)UserRoleType.Patient;

    private static Expression<Func<User, bool>> GetLabByIdFilter(long id)
        => user => user.Id == id && user.Status == EntityStatusConstants.ACTIVE && user.Role == (byte)UserRoleType.Lab;

    private static Expression<Func<User, bool>> GetUserByIdFilter(long id)
        => user => user.Id == id && user.Status == EntityStatusConstants.ACTIVE;

    private static Expression<Func<User, object>> OrderByDescCreatedOn
        => user => user.CreatedOn;

    private static Expression<Func<User, bool>> IsLabDeletableFilter(long labId)
        => user => user.LabId == labId;

    private static Expression<Func<User, bool>> DoctorLabUserFilter(long doctorId)
        => user => user.Role == (byte)UserRoleType.Lab && user.DoctorId == doctorId && user.Status == EntityStatusConstants.ACTIVE;

    #endregion Helper Filters
}