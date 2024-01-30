namespace Entities.DTOs.Request
{
    public class PatientDataDTO
    {
        public int Id { get; set; }

        public DateTimeOffset DOB { get; set; }

        public byte? GenderId { get; set; }

        public string? Gender { get; set; }

        public int Age { get; set; }

        public string ExternalLink { get; set; }

        public string TestTitle { get; set; }

        public TestResultListingRequestDTO TestResults { get; set; } = null!;

    }
}
