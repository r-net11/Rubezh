using StrazhAPI.SKD;
using Infrastructure.Common.TreeList;

namespace StrazhModule.ViewModels
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