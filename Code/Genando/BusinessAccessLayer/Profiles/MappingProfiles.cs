using AutoMapper;
using Common.Enums;
using Entities.DataModels;
using Entities.DTOs.Request;
using Entities.DTOs.Response;
using Microsoft.AspNetCore.Http;
using static BusinessAccessLayer.Implementation.ProfileService;

namespace BusinessAccessLayer.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<ClinicalDetail, ClinicalDetailDTO>()
        .ForMember(dest => dest.Question, opt => opt.MapFrom(src => src.ClinicalQuestions.Question))
        .ReverseMap();

        CreateMap<ClinicalProcess, TimelineDTO>().ReverseMap();

        #region User details form request DTO to User model

        CreateMap<UserDetailsFormRequestDto, User>()
        .ForMember(dest => dest.DOB,
        source => source.MapFrom(src => src.DateOfBirth))
        .ForMember(dest => dest.Role,
        source => source.MapFrom(src =>
        src.IsPatient ? UserRoleType.Patient : UserRoleType.Lab));

        #endregion User details form request DTO to User model

        #region User model to Patient Listing Response DTO

        CreateMap<User, UserListingResponseDto>()
        .ForMember(dest => dest.Name,
        source => source.MapFrom(src => $"{src.FirstName} {src.LastName}"))
        .ForMember(dest => dest.Status,
        source => source.MapFrom(src => PatientConsultationStatusType.New))
        .ForMember(dest => dest.Gender,
        source => source.MapFrom(src => (GenderType)(src.Gender ?? 1)))
        .ForMember(dest => dest.Status,
        source => source.MapFrom(src => src.ConsultationStatus))
        .ForMember(dest => dest.Avatar,
        source => source.MapFrom(src => src.Avatar != null ?
        Convert.ToBase64String(src.Avatar)
        : null));

        #endregion User model to Patient Listing Response DTO

        #region User model => UserDetailsFormResponseDto

        CreateMap<User, UserDetailsFormResponseDto>()
        .ForMember(dest => dest.DateOfBirth,
        source => source.MapFrom(src => src.DOB))
        .ReverseMap()
        .ForMember(dest => dest.DOB,
        source => source.MapFrom(src => src.DateOfBirth))
        .ForMember(dest => dest.Role,
        source => source.MapFrom(src =>
        src.IsPatient ? UserRoleType.Patient : UserRoleType.Lab));

        #endregion User model => UserDetailsFormResponseDto

        CreateMap<PatientListRequestDTO, PageListRequestEntity<ClinicalProcessTest>>()
        .ForMember(dest => dest.PageIndex, opt => opt.MapFrom(src => src.PageIndex >= 1 ? src.PageIndex : 1))
        .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.PageSize >= 1 ? src.PageSize : 10))
        .ForMember(dest => dest.IncludeExpressions, opt => opt.Ignore()); // Ignore IncludeExpressions mapping

        CreateMap<PatientListRequestDTO, PageListRequestEntity<ClinicalProcess>>()
        .ForMember(dest => dest.PageIndex, opt => opt.MapFrom(src => src.PageIndex >= 1 ? src.PageIndex : 1))
        .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.PageSize >= 1 ? src.PageSize : 10))
        .ForMember(dest => dest.IncludeExpressions, opt => opt.Ignore()); // Ignore IncludeExpressions mapping

        // Add any other necessary mapping configurations

        CreateMap<PageListResponseDTO<ClinicalProcessTest>, PageListResponseDTO<PatientInfoDTO>>()
        .ForMember(dest => dest.Records, opt => opt.MapFrom(src => src.Records))
        .ForMember(dest => dest.PageIndex, opt => opt.MapFrom(src => src.PageIndex))
        .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.PageSize))
        .ForMember(dest => dest.TotalRecords, opt => opt.MapFrom(src => src.TotalRecords));

        CreateMap<User, PatientDataDTO>()
        .ForMember(dest => dest.GenderId, opt => opt.MapFrom(src => src.Gender))
        .ReverseMap();

        CreateMap<TestResultDTO, TestResult>()
        .ForMember(dest => dest.ClinicalProcessTestId, opt => opt.MapFrom(src => src.ClinicalProcessTestId))
        .ForMember(dest => dest.ReportAttachmentTitle, opt => opt.MapFrom(src => src.ReportAttachmentTitle))
        .ForMember(dest => dest.LabNotes, opt => opt.MapFrom(src => src.LabNotes))
        .ReverseMap();

        //Mapping for RequestMoreInfoQuestion => PatientQuestionsRequestDto
        CreateMap<RequestMoreInfoQuestion, PatientQuestionsResponseDto>()
        .ReverseMap()
        .ForMember(dest => dest.Id, source => source.Ignore())
        .ForMember(dest => dest.Status, source => source.Ignore());

        //Mapping for RequestMoreInfoQuestion => PatientMoreInfoResponseDto
        CreateMap<RequestMoreInfoQuestion, PatientMoreInfoResponseDto>();

        CreateMap<RequestMoreInfoQuestion, ClinicalDetailDTO>().ReverseMap();

        #region User model to ProfileDetailsDto

        CreateMap<User, UserDetailsInfoDTO>().ReverseMap();

        CreateMap<User, ProfileDetailsDto>().ReverseMap();

        CreateMap<IFormFile, byte[]>().ConvertUsing<FormFileToByteArrayConverter>();

        #endregion

        #region FAQ

        CreateMap<FAQ, FAQResponseDTO>().ReverseMap();

        #endregion

        #region Test_Explanation

        CreateMap<TestDetail, TestDetailInfoDTO>().ReverseMap();

        #endregion

        #region FAQ

        CreateMap<FAQ, FAQResponseDTO>().ReverseMap();

        #endregion

        #region Test_Explanation

        CreateMap<TestDetail, TestDetailInfoDTO>().ReverseMap();
        #endregion

        CreateMap<TestResult, PatientLabResultDTO>()
            .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.ClinicalProcessTests.ClinicalProcesses.Users.Avatar))
            .ForMember(dest => dest.ExternalLink, opt => opt.MapFrom(src => src.ClinicalProcessTests.ClinicalProcesses.ExternalLink))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.ClinicalProcessTests.ClinicalProcesses.Users.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.ClinicalProcessTests.ClinicalProcesses.Users.LastName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.ClinicalProcessTests.ClinicalProcesses.Users.Email))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.ClinicalProcessTests.ClinicalProcesses.Users.Gender))
            .ForMember(dest => dest.DOB, opt => opt.MapFrom(src => src.ClinicalProcessTests.ClinicalProcesses.Users.DOB))
            .ReverseMap();

        CreateMap<TestResult, TestResultListingRequestDTO>()
            .ForMember(dest => dest.ClinicalProcessTestId, opt => opt.MapFrom(src => src.ClinicalProcessTests.Id))
            .ForMember(dest => dest.TestResultId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.TestTitle, opt => opt.MapFrom(src => src.ClinicalProcessTests.TestDetails.Title))
            .ForMember(dest => dest.ReportAttachment, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<TestResult, DownloadLabResultDTO>().ReverseMap();

        CreateMap<ClinicalProcess, PatientTestDetailDTO>()
            .ForMember(dest => dest.DOB, opt => opt.MapFrom(src => src.Users.DOB))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Users.Gender))
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Users.FirstName + " " + src.Users.LastName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Users.Email))
            .ReverseMap();
        #region ClinicalProcessTest => PatientTestInfoResponseDto 

        CreateMap<ClinicalProcessTest, PatientTestInfoResponseDto>()
            .ForMember(dest => dest.Name, source => source.MapFrom(src => $"{src.ClinicalProcesses.Users.FirstName}{src.ClinicalProcesses.Users.LastName}"))
            .ForMember(dest => dest.Id, source => source.MapFrom(src => src.ClinicalProcesses.Users.Id))
            .ForMember(dest => dest.Email, source => source.MapFrom(src => src.ClinicalProcesses.Users.Email))
            .ForMember(dest => dest.DOB, source => source.MapFrom(src => src.ClinicalProcesses.Users.DOB))
            .ForMember(dest => dest.Gender, source => source.MapFrom(src => src.ClinicalProcesses.Users.Gender))
            .ForMember(dest => dest.TestTitle, source => source.MapFrom(src => src.TestDetails!.Title))
            .ForMember(dest => dest.ReferenceCode, source => source.MapFrom(src => src.Id));

        #endregion

        #region Clinical_Question

        CreateMap<ClinicalEnhancement, QuestionEnhancementDTO>()
        .ForMember(dest => dest.Question, opt => opt.MapFrom(src => src.Question))
        .ForMember(dest => dest.TypeOfQuestion, opt => opt.MapFrom(src => src.TypeOfQuestion))
        .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options))
        .ForMember(dest => dest.IsQuestionMandatory, opt => opt.MapFrom(src => src.IsQuestionMandatory))
        .ReverseMap();

        #endregion

        CreateMap<ClinicalEnhancement, ClinicalPathListRequestDTO>()
        .ForMember(dest => dest.TypeOfQuestion, opt => opt.MapFrom(src => src.TypeOfQuestion))
        .ForMember(dest => dest.IsQuestionMandatory, opt => opt.MapFrom(src => src.IsQuestionMandatory == true))
        .ReverseMap();

        #region Contact_Doctor

        CreateMap<User, ContactDoctorDto>()
        .ForMember(dest => dest.Name,
        source => source.MapFrom(src => $"{src.FirstName} {src.LastName}"));

        #endregion

        CreateMap<ClinicalEnhancement, ClinicalDetailDTO>().ReverseMap();

        #region Notification

        CreateMap<Notification, NotificationResultDTO>().ReverseMap();

        #endregion

        #region SwipeActionSetting

        CreateMap<SwipeActionSetting, SwipeActionSettingDTO>().ReverseMap();

        #endregion

    }
}