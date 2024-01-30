using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs.Request
{
    public class SwipeActionSettingDTO
    {
        public long UserId { get; set; }

        public string? SwipeLeftAction { get; set; }

        public string? SwipeRightAction { get; set; }
    }
}
