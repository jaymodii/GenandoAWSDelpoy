using Entities.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs.Request
{
    public class ClinicalAnswerRequestDTO : BaseValidationModel<ClinicalAnswerRequestDTO> 
    {
        public long PatientId { get; set; }

        public bool IsRequestedAnswer { get; set; } = false;
    }
}
