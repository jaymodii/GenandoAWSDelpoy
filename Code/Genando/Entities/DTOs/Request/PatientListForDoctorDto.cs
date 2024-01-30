using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs.Request
{
    public class PatientListForDoctorDto
    {
        public string PatientName { get; set; }
        public byte? Gender { get; set; }
        public string? TestTitle { get; set; }
        public string NextStep { get; set; }
        public DateTimeOffset? ExpectedDate { get; set; }
        public string Status {  get; set; }
        public long PatientId { get; set; }
        public long ClinicalProcessId { get; set; }

    }
}
