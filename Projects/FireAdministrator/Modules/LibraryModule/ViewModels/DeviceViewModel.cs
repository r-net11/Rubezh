using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace LibraryModule.ViewModels
{
    public class DeviceViewModel : BaseViewModel
    {
        public LibraryDevice LibraryDevice { get; private set; }
        public Driver Driver
        {
            get { return LibraryDevice.Driver; }
        }

        public DeviceViewModel(LibraryDevice libraryDevice)
        {
            LibraryDevice = libraryDevice;
        }
    }
}