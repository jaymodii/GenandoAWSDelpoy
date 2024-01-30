using Entities.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs.Response
{
    public class DoctorResultDTO : BaseValidationModel<DoctorResultDTO>
    {
        public long ClinicalProcessTestId { get; set; }

        public string DoctorNotes { get; set; } = null!;
    }
}
