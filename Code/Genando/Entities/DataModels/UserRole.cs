using Entities.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.DataModels
{
    public class UserRole : IdentityEntity<byte>
    {
        [StringLength(16)]
        [Column("title", TypeName = "varchar")]
        public string Title { get; set; } = null!;
    }
}
