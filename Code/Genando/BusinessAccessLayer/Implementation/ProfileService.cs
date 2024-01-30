using AutoMapper;
using BusinessAccessLayer.Abstraction;
using DataAccessLayer.Abstraction;
using Entities.DataModels;
using Entities.DTOs.Request;
using System.Drawing;
using Microsoft.AspNetCore.Http;
using Common.Constants;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using Common.Exceptions;
using Microsoft.AspNetCore.SignalR;
using Common.Hubs;

namespace BusinessAccessLayer.Implementation
{
    public class ProfileService : GenericService<User>, IProfileService
    {
        #region Properties

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IProfileRepository _profileRepository;
        private readonly IHubContext<BroadcastHub> _hubContext;

        #endregion Properties

        #region Constructors

        public ProfileService(IUnitOfWork unitOfWork, IMapper mapper,IProfileRepository profileRepository, IHubContext<BroadcastHub> hubContext) : base(unitOfWork.ProfileRepository, unitOfWork)
        { 
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _profileRepository = profileRepository;
            _hubContext = hubContext;
        }

        #endregion

        #region Interface Methods

        public async Task<UserDetailsInfoDTO> GetProfileDetails(long id)
        {
            User? user = await _unitOfWork.ProfileRepository.GetFirstOrDefaultAsync(user => user.Id == id);

            UserDetailsInfoDTO userDetailsInfoDTO = _mapper.Map<UserDetailsInfoDTO>(user);

            if (user.Avatar != null)
            {
                byte[]? byteData = user.Avatar;

                string imreBase64Data = Convert.ToBase64String(byteData);
                string imgDataURL = string.Format("data:image/png;base64,{0}", imreBase64Data);

                userDetailsInfoDTO.Avatar = imgDataURL;
            }
            return userDetailsInfoDTO;
        }

        public async Task UpdateUserProfile(long id, ProfileDetailsDto profileDetailsDto)
        {
            User? user = await _unitOfWork.ProfileRepository.GetFirstOrDefaultAsync(u => u.Id == id) ?? throw new Exception(MessageConstants.USER_NOT_FOUND);
            bool isEmailExist = await _profileRepository.IsDuplicateEmail(profileDetailsDto.Email, id);

            if (isEmailExist)
                throw new ModelValidationException(MessageConstants.EmailAlreadyExists);

            _mapper.Map(profileDetailsDto, user);

            if (profileDetailsDto.Avatar != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await profileDetailsDto.Avatar.CopyToAsync(memoryStream);
                    user.Avatar = memoryStream.ToArray();
                }
            }

            await UpdateAsync(user);
        }

        public async Task VerifyProfileOtp(long id, string otp)
        {
            User user = await _unitOfWork.ProfileRepository.GetByIdAsync(id);
            
            if (user.OTP != otp || user.ExpiryTime < DateTime.Now)
            {
                await ClearOtpAndExpiry(user);
                throw new ModelValidationException(MessageConstants.Invalidotp);
            }
            await ClearOtpAndExpiry(user);
        }

        private async Task ClearOtpAndExpiry(User user)
        {
            user.OTP = null;
            user.ExpiryTime = null;
            await _unitOfWork.ProfileRepository.UpdateAsync(user);
            await _unitOfWork.SaveAsync();
        }

        public class FormFileToByteArrayConverter : ITypeConverter<IFormFile, byte[]>
        {
            public byte[]? Convert(IFormFile source, byte[] destination, ResolutionContext context)
            {
                if (source == null) return null;

                using (var memoryStream = new MemoryStream())
                {
                    source.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }

        public async Task<IEnumerable<Gender>> GetGenders()
        {
            IEnumerable<Gender> genders = await _unitOfWork.GenderRepository.GetAllAsync(); 
            return genders;
        }

        #endregion
    }
}
