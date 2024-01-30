using Common.Enums;

namespace Entities.DTOs.Response;

public class UserBaseResponseDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTimeOffset DOB { get; set; }
    public GenderType Gender { get; set; }
}

public class UserListingResponseDto : UserBaseResponseDto
{
    public string PhoneNumber { get; set; } = string.Empty;

    public PatientConsultationStatusType Status { get; set; }

    public string? Avatar { get; set; }
}

//Used in showing Request more information feature listing
public class PatientTestInfoResponseDto : UserBaseResponseDto
{
    public string TestTitle { get; set; } = string.Empty;

    public long ReferenceCode { get; set; }
}