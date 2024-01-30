using Entities.DataModels;
using Entities.DTOs.Response;

namespace BusinessAccessLayer.Abstraction
{
    public interface IFAQService : IGenericService<FAQ>
    {
        Task<IEnumerable<FAQResponseDTO>> GetFaq(byte role);
    }
}
