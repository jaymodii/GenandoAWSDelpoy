using Microsoft.AspNetCore.Http;

namespace Entities.DTOs.Request
{
    public class TestResultListingRequestDTO
    {
        public long TestResultId { get; set; }

        public string ReportAttachmentTitle { get; set; } = null!;

        public string? LabNotes { get; set; }

        public IFormFile? ReportAttachment { get; set; } 

        public string? DoctorNotes { get; set; }

        public long ClinicalProcessTestId { get; set; }

        public string TestTitle { get; set; }

        public byte[] ReportAttachmentbyte { get; set; }

    }
}
