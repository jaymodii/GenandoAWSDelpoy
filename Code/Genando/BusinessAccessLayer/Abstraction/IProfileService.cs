using Entities.DataModels;
using Entities.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessAccessLayer.Abstraction
{
    public interface IProfileService : IGenericService<User>
    {
        Task<UserDetailsInfoDTO> GetProfileDetails(long id);
        Task UpdateUserProfile(long id, ProfileDetailsDto profileDetailsDto);
        Task VerifyProfileOtp(long id, string otp);
        Task<IEnumerable<Gender>> GetGenders();
    }
}
