using Common.Utils.Model;
using Entities.DataModels;
using Entities.DTOs.Request;
using Entities.DTOs.Response;

namespace BusinessAccessLayer.Abstraction;

public interface IUserService : IGenericService<User>
{
    Task DeleteUserAsync(long id,
        CancellationToken cancellationToken = default);

    Task EditUserProfile(long id,
        UserDetailsFormRequestDto dto, LoggedUser loggedUser,
        CancellationToken cancellationToken = default);

    Task<bool> IsDuplicateEmail(string email,
        long? userId,
        CancellationToken cancellationToken = default);

    Task<UserListingResponseDto?> LoadDoctorLabUser(long doctorId,
        CancellationToken cancellationToken = default);

    Task<UserDetailsFormResponseDto> LoadUserProfile(long id,
        CancellationToken cancellationToken = default);

    Task<long> RegisterAsync(UserDetailsFormRequestDto dto,
        LoggedUser loggedUser,
        CancellationToken cancellationToken = default);

    Task<PageInformationDto<UserListingResponseDto>> SearchAsync(PatientPageRequestDto dto,
        long doctorId,
        CancellationToken cancellationToken = default);

    Task<ContactDoctorDto> GetDoctorDetails();

    Task<string> GetAvatar(long id);
}