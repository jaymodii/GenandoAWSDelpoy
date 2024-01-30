using Common.Enums;

namespace Entities.DTOs.Request
{
    public class ClinicalPathListRequestDTO
    {
        public long Id { get; set; }

        public string Question { get; set; } = null!;

        public QuestionType TypeOfQuestion { get; set; }

        public bool IsQuestionMandatory { get; set; }

        public string? Options { get; set; }

        public string? Answer { get; set; }
    }
}
