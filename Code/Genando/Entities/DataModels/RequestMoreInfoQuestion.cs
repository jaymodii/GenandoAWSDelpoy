using Common.Enums;
using Entities.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.DataModels;

[Table("requestMoreInfoQuestion")]
public class RequestMoreInfoQuestion : TimestampedEntity<long>
{
    [Column("patient_id")]
    public long PatientId { get; set; }

    [Column("question")]
    [StringLength(255)]
    public string Question { get; set; } = null!;

    [Column("answer")]
    [StringLength(1024)]
    public string? Answer { get; set; }

    [Column("status")]
    public PatientQuestionStatusType Status { get; set; }

    [ForeignKey(nameof(PatientId))]
    public virtual User Patient { get; set; } = null!;
}
