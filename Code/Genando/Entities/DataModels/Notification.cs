using Entities.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.DataModels
{
    public class Notification : TimestampedEntity<long>
    {
        [Column("sent_by")]
        public long SentBy { get; set; }

        [Column("send_to")]
        public long SendTo { get; set; }

        [Column("notification_message")]
        [StringLength(255)]
        public string NotificationMessage { get; set; } = null!;

        [Column("has_read")]
        public bool HasRead { get; set; }

        [Column("is_temp_deleted")]
        public bool IsTempDeleted { get; set; }

        [ForeignKey(nameof(SentBy))]
        public virtual User SentByUser { get; set; } = null!;

        [ForeignKey(nameof(SendTo))]
        public virtual User SendToUser { get; set; } = null!;
    }
}
