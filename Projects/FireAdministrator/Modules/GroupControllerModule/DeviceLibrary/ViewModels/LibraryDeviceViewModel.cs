using RubezhAPI.GK;
using System.Linq;
using Infrastructure.Common.Windows.TreeList;

namespace GKModule.ViewModels
{
	public class LibraryDeviceViewModel : TreeNodeViewModel<LibraryDeviceViewModel>
	{
		public GKLibraryDevice LibraryDevice { get; private set; }
		public GKDriver Driver
		{
			get { return LibraryDevice.Driver; }
		}

		public LibraryDeviceViewModel(GKLibraryDevice gkLibraryDevice)
		{
			LibraryDevice = gkLibraryDevice;
		}
	}
}