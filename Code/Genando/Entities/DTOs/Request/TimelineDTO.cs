using System.ComponentModel.DataAnnotations;

namespace Entities.DTOs.Request
{
    public class TimelineDTO
    {
        [Required]
        public byte Status { get; set; }
    }
}
