
using System.ComponentModel.DataAnnotations;

namespace Entities.DTOs.Request
{
    public class NotificationResultDTO
    {
        public long Id { get; set; }

        public long SentBy { get; set; }

        public long SendTo { get; set; }

        public string NotificationMessage { get; set; } = null!;

        public bool HasRead { get; set; }

        public string Time { get; set; }

        public bool IsTempDeleted { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
    }
}
