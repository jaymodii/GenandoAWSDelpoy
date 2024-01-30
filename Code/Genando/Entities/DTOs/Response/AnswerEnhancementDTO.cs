using Entities.Abstract;
using System.ComponentModel.DataAnnotations;

namespace Entities.DTOs.Response
{
    public class AnswerEnhancementDTO : BaseValidationModel<AnswerEnhancementDTO>
    {
        public long Id { get; set; }

        public string Answer { get; set; } = null!;
    }
}