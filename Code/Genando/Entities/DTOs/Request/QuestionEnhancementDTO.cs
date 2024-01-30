using Common.Enums;
using Entities.Abstract;
using System.ComponentModel.DataAnnotations;

namespace Entities.DTOs.Request
{
    public class QuestionEnhancementDTO:BaseValidationModel<QuestionEnhancementDTO>
    {
        public long? QuestionId { get; set; }    

        public string Question { get; set; } = null!;

        [EnumDataType(typeof(QuestionType))]
        public QuestionType TypeOfQuestion { get; set; }

        public string? Options { get; set; }

        public bool IsQuestionMandatory { get; set; } = false;
    }
}
