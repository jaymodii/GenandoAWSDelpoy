using Entities.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.DataModels
{
    public class TestDetail : IdentityEntity<long>
    {
        [StringLength(10)]
        [Column("abbreviation", TypeName = "varchar")]
        public string Abbreviation { get; set; } = null!;

        [StringLength(255)]
        [Column("title", TypeName = "varchar")]
        public string Title { get; set; } = null!;

        [StringLength(2048)]
        [Column("description", TypeName = "varchar")]
        public string? Description { get; set; }

        [Column("price")]
        public double Price { get; set; }

        [Column("duration")]
        public byte Duration { get; set; }
    }
}
