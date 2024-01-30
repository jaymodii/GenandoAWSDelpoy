using Common.Enums;
using Entities.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.DataModels
{
    public class ClinicalEnhancement : TimestampedEntity<long>
    {
        [Column("patient_id")]
        public long? PatientId { get; set; }

        [Column("doctor_id")]
        public long? DoctorId { get; set; }

        [Column("level_of_question")]
        public QuestionLevel LevelOfQuestion { get; set; }

        [Column("question")]
        [StringLength(255)]
        public string Question { get; set; } = null!;

        [Column("type_of_question")]
        public QuestionType TypeOfQuestion { get; set; }

        [Column("options")]
        [StringLength(1024)]
        public string? Options { get; set; }

        [Column("isQuestionMandatory")]
        public bool IsQuestionMandatory { get; set; }

        [ForeignKey(nameof(PatientId))]
        public virtual User? Patient { get; set; } = null!;  

        [ForeignKey(nameof(DoctorId))]
        public virtual User? Doctor{ get; set; }
    }
}
