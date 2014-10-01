using FiresecAPI.GK;
using Infrastructure.Common.TreeList;

namespace GKModule.ViewModels
{
	public class XDeviceViewModel : TreeNodeViewModel<XDeviceViewModel>
	{
		public GKLibraryDevice LibraryDevice { get; private set; }
		public XDriver Driver
		{
			get { return LibraryDevice.Driver; }
		}
		public XDeviceViewModel(GKLibraryDevice gkLibraryDevice)
		{
			LibraryDevice = gkLibraryDevice;
		}
	}
}