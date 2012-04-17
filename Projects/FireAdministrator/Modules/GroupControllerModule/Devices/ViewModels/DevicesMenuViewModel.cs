
namespace GKModule.ViewModels
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