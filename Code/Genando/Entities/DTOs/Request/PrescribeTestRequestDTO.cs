using Entities.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs.Request
{
    public class PrescribeTestRequestDTO:BaseValidationModel<PrescribeTestRequestDTO>
    {
        public long ClinicalProcessId { get; set; }

        public List<long> TestIds { get; set; }
    }
}
