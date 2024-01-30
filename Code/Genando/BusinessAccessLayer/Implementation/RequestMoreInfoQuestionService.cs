using AutoMapper;
using BusinessAccessLayer.Abstraction;
using Common.Enums;
using Common.Hubs;
using Common.Utils.Model;
using DataAccessLayer.Abstraction;
using Entities.DataModels;
using Entities.DTOs.Request;
using Entities.DTOs.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;

namespace BusinessAccessLayer.Implementation;

public class RequestMoreInfoQuestionService
    : GenericService<RequestMoreInfoQuestion>, IRequestMoreInfoQuestionService
{
    #region Properties and fields

    private readonly IUnitOfWork _unitOfWork;
    private readonly IRequestMoreInfoQuestionRepository _questionRepository;
    private readonly IMapper _mapper;
    private JwtManageService _jwtManageService;
    private readonly IHubContext<BroadcastHub> _hubContext;

    #endregion Properties and fields

    #region Constructor

    public RequestMoreInfoQuestionService(IUnitOfWork unitOfWork,
        IRequestMoreInfoQuestionRepository questionRepository,
        IConfiguration config,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper, IHubContext<BroadcastHub> hubContext)
        : base(questionRepository, unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _questionRepository = questionRepository;
        _jwtManageService = new(config, httpContextAccessor);
        _mapper = mapper;
        _hubContext = hubContext;
    }

    #endregion Constructor

    #region Interface methods

    public async Task<IEnumerable<PatientQuestionsResponseDto>> GetPatientQuestions(long patientId,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<RequestMoreInfoQuestion> models
            = await GetAllAsync(filter: DraftQuestionFilter(patientId));

        return _mapper.Map<IEnumerable<PatientQuestionsResponseDto>>(models);
    }

    public async Task SetPatientQuestions(long id,
        PatientMoreQuestionRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        LoggedUser loggedUser = _jwtManageService.GetLoggedUser();

        await DeletePatientQuestions(id, dto.DeletedQuestions, cancellationToken);

        await UpdatePatientQuestions(id, dto, cancellationToken);

        Notification notification = new Notification()
        {
            SentBy = loggedUser.UserId,
            SendTo = id,
            NotificationMessage = $"Dr. {loggedUser.Name} has requested for additional information from you. Please respond with the required details."
        };

        await _unitOfWork.NotificationRepository.AddAsync(notification);

        await _hubContext.Clients.All.SendAsync("BroadcastMessage");

        await _unitOfWork.SaveAsync(cancellationToken);
    }

    public async Task<IEnumerable<PatientMoreInfoResponseDto>> LoadPatientMoreQuestions(CancellationToken cancellationToken = default)
    {
        long patientId = _jwtManageService.GetLoggedUser().UserId;
        IEnumerable<RequestMoreInfoQuestion> models = await GetAllAsync(filter: PublishPatientDraftQuestionFilter(patientId));

        return _mapper.Map<IEnumerable<PatientMoreInfoResponseDto>>(models);
    }

    public async Task SavePatientMoreQuestion(PatientMoreInfoRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        long patientId = _jwtManageService.GetLoggedUser().UserId;

        List<RequestMoreInfoQuestion> models = (await GetAllAsync(PublishPatientDraftQuestionFilter(patientId))).ToList();

        int questionsCount = dto.PatientMoreInfo.Count;
        for (int i = 0; i < questionsCount; i++)
        {
            models[i].Answer = dto.PatientMoreInfo[i].Answer;
            models[i].Status = dto.Status;
        }

        await UpdateRangeAsync(models, cancellationToken);

        User? user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(user => user.Id == patientId);

        Notification notification = new Notification()
        {
            SentBy = patientId,
            SendTo = (long)user.DoctorId,
            NotificationMessage = $"{user.FirstName} {user.LastName} has provided answers to your additional information. Please review it."
        };

        await _unitOfWork.NotificationRepository.AddAsync(notification);

        await _hubContext.Clients.All.SendAsync("BroadcastMessage");

        await _unitOfWork.SaveAsync(cancellationToken);
    }

    #endregion Interface methods

    #region Helper Methods

    private async Task<IEnumerable<RequestMoreInfoQuestion>> GetPublishedQuestions(long patientId)
        => await GetAllAsync(filter: PublishQuestionFilter(patientId));

    public async Task DeletePatientQuestions(long id,
        List<long> deletedQuestionsId,
        CancellationToken cancellationToken = default)
    {
        if (deletedQuestionsId.Count == 0) return;

        await _questionRepository.DeletePatientQuestions(id, deletedQuestionsId, cancellationToken);
    }

    private async Task UpdatePatientQuestions(long id,
        PatientMoreQuestionRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        List<RequestMoreInfoQuestion> models = (await GetAllAsync(filter: UserQuestionFilter(id))).ToList();

        if (models.Count == 0)
        {
            await AddPatientQuestions(id, dto.Status, dto.Questions, cancellationToken);
            return;
        }

        IEnumerable<RequestMoreInfoQuestion> updateQuestions = models
            .Where(model => dto.QuestionIds.Exists(id => id == model.Id));

        foreach (RequestMoreInfoQuestion question in updateQuestions)
        {
            PatientQuestionsResponseDto requestQuestion = dto.Questions.First(dto => dto.Id == question.Id);

            question.Question = requestQuestion.Question;
            question.Status = dto.Status;
        }

        IEnumerable<PatientQuestionsResponseDto> addQuestions
            = dto.Questions
                .Where(question
                => !models.Exists(model => model.Id == question.Id));

        await AddPatientQuestions(id, dto.Status, addQuestions, cancellationToken);

        if (updateQuestions.Any())
            await UpdateRangeAsync(updateQuestions, cancellationToken);

    }

    private async Task AddPatientQuestions(long id,
        PatientQuestionStatusType status,
        IEnumerable<PatientQuestionsResponseDto> addQuestions,
        CancellationToken cancellationToken)
    {
        IEnumerable<RequestMoreInfoQuestion> addQuestionModels = _mapper.Map<IEnumerable<RequestMoreInfoQuestion>>(addQuestions,
            opt => opt.AfterMap(
                (src, dest) =>
                {
                    foreach (RequestMoreInfoQuestion model in dest)
                    {
                        model.Status = status;
                        model.PatientId = id;
                    }
                }));

        await AddRangeAsync(addQuestionModels, cancellationToken);
    }

    #endregion Helper Methods

    #region Helper Filters

    private static Expression<Func<RequestMoreInfoQuestion, bool>> DraftQuestionFilter(long patientId)
        => requestMoreInfoQuestion
        => requestMoreInfoQuestion.PatientId == patientId
            && requestMoreInfoQuestion.Status == PatientQuestionStatusType.DraftByDoctor;

    private static Expression<Func<RequestMoreInfoQuestion, bool>> PublishQuestionFilter(long patientId)
    => requestMoreInfoQuestion
    => requestMoreInfoQuestion.PatientId == patientId
        && requestMoreInfoQuestion.Status == PatientQuestionStatusType.PublishedByDoctor;

    private static Expression<Func<RequestMoreInfoQuestion, bool>> UserQuestionFilter(long patientId)
        => requestMoreInfoQuestions
        => requestMoreInfoQuestions.PatientId == patientId
            && (int)requestMoreInfoQuestions.Status != 0;

    private static Expression<Func<RequestMoreInfoQuestion, bool>> PublishPatientDraftQuestionFilter(long patientId)
        => requestMoreInfoQuestion
        => requestMoreInfoQuestion.PatientId == patientId
            && (requestMoreInfoQuestion.Status == PatientQuestionStatusType.PublishedByDoctor
                || requestMoreInfoQuestion.Status == PatientQuestionStatusType.DraftByPatient);

    #endregion Helper Filters
}