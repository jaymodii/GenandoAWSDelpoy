using Entities.Abstract;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.DataModels
{
    public class ClinicalProcessTest : AuditableEntity<long>
    {
        [Column("clinical_process_id")]
        public long ClinicalProcessId { get; set; }

        [Column("test_id")]
        public long TestId { get; set; }

        [ForeignKey(nameof(ClinicalProcessId))]
        public ClinicalProcess ClinicalProcesses { get; set; } = null!;

        [ForeignKey(nameof(TestId))]
        public TestDetail TestDetails { get; set; } = null!;
    }
}
