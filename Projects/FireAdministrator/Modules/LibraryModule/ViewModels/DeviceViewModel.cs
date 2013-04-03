using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure;

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

		public string AlternativeName
		{
			get { return LibraryDevice.AlternativeName; }
			set
			{
				LibraryDevice.AlternativeName = value;
				//OnPropertyChanged("AlternativeName");
				OnPropertyChanged("LibraryDevice");
				ServiceFactory.SaveService.LibraryChanged = true;
			}
		}
    }
}