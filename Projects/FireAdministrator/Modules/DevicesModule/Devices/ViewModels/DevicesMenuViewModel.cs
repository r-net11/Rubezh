using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class DevicesMenuViewModel
    {
        public DevicesMenuViewModel(DevicesViewModel devicesViewModel)
        {
            Context = devicesViewModel;
        }

        public DevicesViewModel Context { get; private set; }
    }
}