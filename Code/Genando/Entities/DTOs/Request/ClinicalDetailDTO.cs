using System.ComponentModel.DataAnnotations;

namespace Entities.DTOs.Request
{
    public class ClinicalDetailDTO
    {
        public long Id { get; set; }

        public string? Question { get; set; }

        public string? Answer { get; set; }
    }
}
