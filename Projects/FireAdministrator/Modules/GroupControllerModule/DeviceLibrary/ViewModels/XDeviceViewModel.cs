using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class XDeviceViewModel : BaseViewModel
	{
		public LibraryXDevice LibraryDevice { get; private set; }
		public XDriver Driver
		{
			get { return LibraryDevice.Driver; }
		}
		public XDevice Device { get; private set; }
		public XDeviceViewModel(XDevice device)
		{
			Device = device;
		}
		public XDeviceViewModel(LibraryXDevice libraryXDevice)
		{
			LibraryDevice = libraryXDevice;
		}
	}
}