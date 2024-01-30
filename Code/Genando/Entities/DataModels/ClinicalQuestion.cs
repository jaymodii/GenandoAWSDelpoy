using Common.Enums;
using Entities.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.DataModels
{
    public class ClinicalQuestion : IdentityEntity<int>
    {
        [StringLength(255)]
        [Column("question", TypeName = "varchar")]
        public string Question { get; set; } = null!;

        //[Column("question_type")]
        //public QuestionType QuestionType { get; set; }

    }
}
