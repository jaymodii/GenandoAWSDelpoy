using Common.Constants;

namespace Common.Enums;
public enum PatientQuestionStatusType : byte
{
    Deleted = EntityStatusConstants.DELETED,
    DraftByDoctor = 1,
    PublishedByDoctor = 2,
    DraftByPatient = 3,
    PublishedByPatient = 4
}
