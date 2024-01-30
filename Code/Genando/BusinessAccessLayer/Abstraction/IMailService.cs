using Entities.DTOs.Request;
using Entities.DTOs.Response;
using Microsoft.AspNetCore.Http;

namespace BusinessAccessLayer.Abstraction
{
    public interface IMailService
    {
        Task SendMailAsync(MailDto mailData, CancellationToken cancellationToken = default);

        Task<IFormFile> ConvertByteToFormFile(byte[] byteFile,string fileName);

        Task<List<IFormFile>> ConvertByteListToFormFiles(FileConversionDTO fileConversionDTO);
    }
}
