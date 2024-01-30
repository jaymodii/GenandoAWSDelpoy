using Entities.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.DataModels
{
    public class UserStatus : IdentityEntity<byte>
    {
        [StringLength(32)]
        [Column("title", TypeName = "varchar")]
        public string Title { get; set; } = null!;
    }
}
