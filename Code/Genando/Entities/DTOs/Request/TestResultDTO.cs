using Entities.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Entities.DTOs.Request
{
    public class TestResultDTO : BaseValidationModel<TestResultDTO>
    {
        public long ClinicalProcessTestId { get; set; }

        public string ExternalLink { get; set; } = null!;

        public long TestResultId { get; set; }

        public string ReportAttachmentTitle { get; set; } = null!;

        public string? LabNotes { get; set; }

        public IFormFile? ReportAttachment { get; set; }
    }
}

