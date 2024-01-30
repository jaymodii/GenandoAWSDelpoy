using Entities.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.DataModels
{
    public class ClinicalProcess : AuditableEntity<long>
    {
        [Column("patient_id")]
        public long PatientId { get; set; }

        [Column("external_link", TypeName = "varchar")]
        [StringLength(255)]
        public string? ExternalLink { get; set; }

        [Column("status")]
        public byte Status { get; set; }

        [Column("next_step")]
        public byte NextStep { get; set; }

        [Column("deadline")]
        public DateTimeOffset? Deadline { get; set; }

        [Column("expected_date")]
        public DateTimeOffset? ExpectedDate { get; set; }

        [ForeignKey(nameof(PatientId))]
        public User Users { get; set; } = null!;

        [ForeignKey(nameof(Status))]
        public ClinicalProcessStatus ClinicalProcessStatuses { get; set; } = null!;

        [ForeignKey(nameof(NextStep))]
        public ClinicalProcessStatus NextStepClinicalProcess { get; set; } = null!;
    }
}
