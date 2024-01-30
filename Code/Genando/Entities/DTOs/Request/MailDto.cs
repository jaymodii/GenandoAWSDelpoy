using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs.Request
{
    public class MailDto
    {
        public string ToEmail { get; set; } = String.Empty;

        public string Subject { get; set; } = String.Empty;

        public string Body { get; set; } = String.Empty;

        public List<IFormFile> Attachments { get; set; } = new List<IFormFile>();
    }
    public class MailSettingDto
    {
        public string? Mail { get; set; }

        public string? DisplayName { get; set; }

        public string? Password { get; set; }

        public string? Host { get; set; }

        public int Port { get; set; }
    }
}
