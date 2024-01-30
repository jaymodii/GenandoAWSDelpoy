using Common.Enums;
using Entities.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.DataModels
{
    public class ClinicalDetail : TimestampedEntity<long>
    {
        [Column("patient_id")]
        public long PatientId { get; set; }

        [Column("question_id")]
        public int QuestionId { get; set; }

        [Column("answer", TypeName = "varchar")]
        [StringLength(1024)]
        public string? Answer { get; set; }

        [ForeignKey(nameof(PatientId))]
        public User Users { get; set; } = null!;

        [ForeignKey(nameof(QuestionId))]
        public ClinicalQuestion ClinicalQuestions { get; set; } = null!;
    }
}
