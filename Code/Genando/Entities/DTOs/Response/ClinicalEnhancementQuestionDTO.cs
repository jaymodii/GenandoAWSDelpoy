using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs.Response
{
    public class ClinicalEnhancementQuestionDTO
    {
        public long? QuestionId { get; set; }
        public string? Question { get; set; }
        public QuestionType TypeOfQuestion { get; set; }
        public string? Options { get; set; }
        public bool IsQuestionMandatory { get; set; }
    }
}
