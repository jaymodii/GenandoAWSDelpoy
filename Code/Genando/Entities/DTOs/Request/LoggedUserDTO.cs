using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs.Request
{
    public class LoggedUserDTO
    {
        public long UserId { get; set; }
        public int Role { get; set;}
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public long LabId { get; set; } 
    }
}
