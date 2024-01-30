using Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs.Response
{
    public class PatientTestDetailDTO
    {
        public IEnumerable<TestDetail> testDetails { get; set; } = null!;

        public string PatientName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public byte Gender { get; set; }

        public DateTimeOffset? DOB { get; set; }

        public string Avatar { get; set; } = string.Empty;

        public List<long> prescribedTestId { get; set; }
    }
}
