using System.ComponentModel.DataAnnotations;

namespace Entities.DTOs.Response
{
    public class ClinicalAnswerDTO
    {
        public long Id { get; set; }

        public string Answer { get; set; } = null!;
    }
}
