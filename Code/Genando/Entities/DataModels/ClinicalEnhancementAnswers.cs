using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Abstract;

namespace Entities.DataModels
{
    public class ClinicalEnhancementAnswer : TimestampedEntity<long>
    {
        [Column("patient_id")]
        public long PatientId { get; set; }

        [Column("question_id")]
        public long QuestionId { get; set; }

        [Column("answer")]
        [StringLength(1024)]
        public string? Answer { get; set; }

        [ForeignKey(nameof(PatientId))]
        public virtual User Patient { get; set; } = null!;

        [ForeignKey(nameof(QuestionId))]
        public virtual ClinicalEnhancement Question { get; set; } = null!;
    }
}
