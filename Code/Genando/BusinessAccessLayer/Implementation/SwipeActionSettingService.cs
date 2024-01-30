using AutoMapper;
using BusinessAccessLayer.Abstraction;
using DataAccessLayer.Abstraction;
using Entities.DataModels;
using Entities.DTOs.Request;

namespace BusinessAccessLayer.Implementation
{
    public class SwipeActionSettingService : GenericService<SwipeActionSetting>, ISwipeActionSettingService
    {
        #region Properties

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        #endregion Properties

        #region Constructors

        public SwipeActionSettingService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork.SwipeActionSettingRepository, unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #endregion Constructors

        public async Task<SwipeActionSettingDTO> GetSwipeSettingAsync(long userId)
        {
            SwipeActionSetting? swipeActionSetting = await _unitOfWork.SwipeActionSettingRepository.GetFirstOrDefaultAsync(swipeSetting => swipeSetting.UserId == userId);

            SwipeActionSettingDTO swipeActionSettingDTO = _mapper.Map<SwipeActionSettingDTO>(swipeActionSetting);

            return swipeActionSettingDTO;
        }

        public async Task SaveSwipeSettingAsync(long userId, SwipeActionSettingDTO swipeActionSettingDTO)
        {
            SwipeActionSetting? swipeActionSetting = await _unitOfWork.SwipeActionSettingRepository.GetFirstOrDefaultAsync(swipeSetting => swipeSetting.UserId == userId);
            if (swipeActionSetting != null)
            {
                swipeActionSetting.SwipeRightAction = swipeActionSettingDTO.SwipeRightAction;
                swipeActionSetting.SwipeLeftAction = swipeActionSettingDTO.SwipeLeftAction;
                await UpdateAsync(swipeActionSetting);
            }
        }
    }
}
