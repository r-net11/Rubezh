using FiresecAPI.Models;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class TelephoneLineDetailsViewModel : SaveCancelDialogContent
    {
        Device _device;

        public TelephoneLineDetailsViewModel(Device device)
        {
            Title = "Параметры МС-ТЛ";
            _device = device;
        }
    }
}
