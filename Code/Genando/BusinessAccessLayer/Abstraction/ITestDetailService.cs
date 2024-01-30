using Entities.DataModels;
using Entities.DTOs.Request;

namespace BusinessAccessLayer.Abstraction
{
    public interface ITestDetailService : IGenericService<TestDetail>
    {
        Task<IEnumerable<TestDetailInfoDTO>> GetTestDetails();
    }
}
