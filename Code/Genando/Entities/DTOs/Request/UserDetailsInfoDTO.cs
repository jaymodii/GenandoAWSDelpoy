using Common.Enums;
using System.Drawing;

namespace Entities.DTOs.Request
{
    public class UserDetailsInfoDTO
    {

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Headline { get; set; }
        public DateTimeOffset? DOB { get; set; } 
        public GenderType? Gender { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? Avatar { get; set; }
    }
}
