using Entities.DataModels;
using Entities.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessAccessLayer.Abstraction
{
    public interface ISwipeActionSettingService : IGenericService<SwipeActionSetting>
    {
        Task<SwipeActionSettingDTO> GetSwipeSettingAsync(long userId);

        Task SaveSwipeSettingAsync(long userId, SwipeActionSettingDTO swipeActionSettingDTO);
    }
}
