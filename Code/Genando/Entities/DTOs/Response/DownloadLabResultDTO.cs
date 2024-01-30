using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs.Response
{
    public class DownloadLabResultDTO
    {
        public string ReportAttachmentTitle { get; set; } = null!;
        public byte[] ReportAttachment { get; set; } = null!;
        public string ContentType { get; set; } = "application/pdf";
    }
}
