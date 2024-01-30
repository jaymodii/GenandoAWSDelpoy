using Entities.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.DataModels
{
    public class FAQ : IdentityEntity<int>
    {
        [StringLength(124)]
        [Column("question", TypeName = "varchar")]
        public string Question { get; set; } = null!;

        [StringLength(2048)]
        [Column("answer", TypeName = "varchar")]
        public string Answer { get; set; } = null!;

        [Column("for_whom")]
        public byte ForWhom { get; set; }

        [ForeignKey(nameof(ForWhom))]
        public UserRole UserRoles { get; set; } = null!;
    }
}
