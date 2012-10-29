using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
    public class XDeviceViewModel : BaseViewModel
    {
        public LibraryXDevice LibraryXDevice { get; private set; }
        public XDriver XDriver
        {
            get { return LibraryXDevice.XDriver; }
        }

        public XDeviceViewModel(LibraryXDevice libraryXDevice)
        {
            LibraryXDevice = libraryXDevice;
        }
    }
}