using AutoMapper;
using BusinessAccessLayer.Abstraction;
using DataAccessLayer.Abstraction;
using Entities.DataModels;
using Entities.DTOs.Response;

namespace BusinessAccessLayer.Implementation
{
    public class FAQService : GenericService<FAQ>, IFAQService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public FAQService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork.FAQRepository, unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FAQResponseDTO>> GetFaq(byte role)
        {
            List<FAQ> faq = await _unitOfWork.FAQRepository.GetAllAsync(faq => faq.ForWhom == role);

            List<FAQResponseDTO> faqResponseDTO = _mapper.Map<List<FAQResponseDTO>>(faq).ToList();
            return faqResponseDTO;
        }
    }
}