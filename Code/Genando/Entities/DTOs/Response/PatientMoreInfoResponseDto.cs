using Common.Enums;
using Entities.Abstract;

namespace Entities.DTOs.Response;
public class PatientMoreInfoResponseDto
{
    public long Id { get; set; }

    public string Question { get; set; } = string.Empty;

    public string? Answer { get; set; }
}

public class PatientMoreInfoRequestDto
    : BaseValidationModel<PatientMoreInfoRequestDto>
{
    public List<PatientMoreInfoResponseDto> PatientMoreInfo { get; set; } = new();

    public PatientQuestionStatusType Status { get; set; }
}