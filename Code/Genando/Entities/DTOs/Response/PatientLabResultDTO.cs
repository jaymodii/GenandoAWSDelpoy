using Entities.DTOs.Request;

namespace Entities.DTOs.Response
{
    public class PatientLabResultDTO
    {
        public List<TestResultListingRequestDTO> TestResults { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public byte? Gender { get; set; }

        public DateTimeOffset? DOB { get; set; }

        public string? ExternalLink { get; set; }

        public string? Avatar { get; set; }
    }
}
