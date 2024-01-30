using Common.Utils;
using Entities.DataModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Abstract;
public abstract class AuditableEntity<T> : IdentityEntity<T>
{
    [Column("created_on")]
    public DateTimeOffset CreatedOn { get; set; } = DateUtil.UtcNow;

    [Column("updated_on")]
    public DateTimeOffset? UpdatedOn { get; set; } = DateUtil.UtcNow;

    [Column("created_by")]
    public long? CreatedBy { get; set; }

    [Column("updated_by")]
    public long? UpdatedBy { get; set; }

    [ForeignKey(nameof(CreatedBy))]
    public virtual User? CreatedByUser { get; set; }

    [ForeignKey(nameof(UpdatedBy))]
    public virtual User? UpdatedByUser { get; set; }
}
