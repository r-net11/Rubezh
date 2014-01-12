using Infrastructure.Common.TreeList;
using XFiresecAPI;

namespace SkudModule.ViewModels
{
	public class LibraryDeviceViewModel : TreeNodeViewModel<LibraryDeviceViewModel>
	{
		public SKDLibraryDevice LibraryDevice { get; private set; }
		public SKDDriver Driver
		{
			get { return LibraryDevice.Driver; }
		}
		public LibraryDeviceViewModel(SKDLibraryDevice skdLibraryDevice)
		{
			LibraryDevice = skdLibraryDevice;
		}
	}
}