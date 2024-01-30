using Entities.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataModels
{
    public class SwipeActionSetting : IdentityEntity<long>
    {
        [Column("user_Id")]
        public long UserId { get; set; }

        [Column("swipe_left_action")]
        public string? SwipeLeftAction { get; set; }

        [Column("swipe_right_action")]
        public string? SwipeRightAction { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User user { get; set; } = null!;
    }
}
