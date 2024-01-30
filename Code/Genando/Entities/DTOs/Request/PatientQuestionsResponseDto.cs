using Common.Enums;
using Entities.Abstract;

namespace Entities.DTOs.Request;
public class PatientQuestionsResponseDto
{
    public long Id { get; set; }

    public string Question { get; set; } = string.Empty;

    public PatientQuestionStatusType Status { get; set; }
}

public class PatientMoreQuestionRequestDto
    : BaseValidationModel<PatientMoreQuestionRequestDto>
{
    public List<PatientQuestionsResponseDto> Questions { get; set; } = new();

    public PatientQuestionStatusType Status { get; set; }

    public List<long> DeletedQuestions { get; set; } = new();

    public List<long> QuestionIds { get; set; } = new();
}