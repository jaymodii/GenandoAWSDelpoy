using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs.Request
{
    public class PatientDetailsDto
    {
        public string? Name { get; set; }
        public string? Avatar { get; set; }
        public byte? Gender { get; set; }

        public string? Email { get; set; }

        public DateTimeOffset? DOB{ get; set; }

        public string? TestName { get; set; }

        public double TestPrice{ get; set; }

        public DateTimeOffset? EstimatedDate { get; set; }

        public List<long> PrescribedTestIds { get; set; } = null!;
    }
}
