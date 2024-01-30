using Common.Enums;
using Entities.Abstract;

namespace Entities.DTOs.Request;

public class UserDetailsFormRequestDto
    : BaseValidationModel<UserDetailsFormRequestDto>
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTimeOffset? DateOfBirth { get; set; }

    public string PhoneNumber { get; set; } = string.Empty;

    public GenderType? Gender { get; set; }

    public string Address { get; set; } = string.Empty;

    public bool IsPatient { get; set; } = true;
}

public class UserDetailsFormResponseDto
    : UserDetailsFormRequestDto
{
    public long Id { get; set; }
}
