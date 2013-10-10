using Infrastructure.Common.TreeList;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class XDeviceViewModel : TreeNodeViewModel<XDeviceViewModel>
	{
		public LibraryXDevice LibraryDevice { get; private set; }
		public XDriver Driver
		{
			get { return LibraryDevice.Driver; }
		}
		public XDeviceViewModel(LibraryXDevice libraryXDevice)
		{
			LibraryDevice = libraryXDevice;
		}
	}
}