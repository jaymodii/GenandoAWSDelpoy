using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs.Request
{
    public class PatientInfoDTO
    {
        public long ClinicalProcessTestId { get; set; }

        public long PatientId { get; set; }

        public string? Test { get; set;}

        public string? Notes { get; set; }

        public DateTimeOffset? Deadline { get; set; }

        public bool isResultUploaded { get; set; } = false;

        public bool isSampleRecieve { get; set; } = false;

        public bool isResultsPublished { get; set; } = false;

    }
}
