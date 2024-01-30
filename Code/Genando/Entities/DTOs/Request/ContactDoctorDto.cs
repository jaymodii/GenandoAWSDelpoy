using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs.Request
{
    public class ContactDoctorDto
    {
        public string Name { get; set; } = null!;
        
        public string PhoneNumber { get; set; }= null!;

        public string Email { get; set; } = null!;  
    }
}
