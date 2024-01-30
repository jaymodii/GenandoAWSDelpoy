using AutoMapper;
using BusinessAccessLayer.Abstraction;
using DataAccessLayer.Abstraction;
using Entities.DataModels;
using Entities.DTOs.Request;

namespace BusinessAccessLayer.Implementation
{
    public class TestDetailService : GenericService<TestDetail>, ITestDetailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TestDetailService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork.TestDetailRepository, unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TestDetailInfoDTO>> GetTestDetails()
        {
            IEnumerable<TestDetail> testDetails = await _unitOfWork.TestDetailRepository.GetAllAsync();

            IEnumerable<TestDetailInfoDTO> testDetailInfoDTO = _mapper.Map<IEnumerable<TestDetailInfoDTO>>(testDetails);

            return testDetailInfoDTO;
        }
    }
}
