using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utils.Model
{
    public class JwtSetting
    {
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public int ExpiryMinutes { get; set; }
        public string Key { get; set; } = null!;
    }
}
