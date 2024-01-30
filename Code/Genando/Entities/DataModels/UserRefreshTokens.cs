using Entities.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataModels
{
    public class UserRefreshTokens : AuditableEntity<long>
    {
        [Column("email")]
        public string Email { get; set; } = null!;

        [Column("refresh_token")]
        public string RefreshToken { get; set; } = null!;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;
    }
}
