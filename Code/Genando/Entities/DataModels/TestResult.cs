using Entities.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.DataModels
{
    public class TestResult : AuditableEntity<long>
    {
        [Column("clinical_process_test_id")]
        public long ClinicalProcessTestId { get; set; }

        [Column("report_attachment_title", TypeName = "varchar")]
        [StringLength(100)]
        public string ReportAttachmentTitle { get; set; } = null!;

        [Column("report_attachment")]
        public byte[] ReportAttachment { get; set; } = null!;

        [Column("doctor_notes", TypeName = "varchar")]
        [StringLength(512)]
        public string? DoctorNotes { get; set; }

        [StringLength(512)]
        [Column("lab_notes", TypeName = "varchar")]
        public string? LabNotes { get; set; }

        [ForeignKey(nameof(ClinicalProcessTestId))]
        public ClinicalProcessTest ClinicalProcessTests { get; set; } = null!;
    }
}
