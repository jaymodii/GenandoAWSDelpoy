using Common.Enums;
using Entities.Abstract;

namespace Entities.DTOs.Request;

public class PatientPageRequestDto : BaseValidationModel<PatientPageRequestDto>
{
    public PageRequestDto? PageRequest { get; set; }

    public GenderType? Gender { get; set; }

    public PatientConsultationStatusType? Status { get; set; }
}