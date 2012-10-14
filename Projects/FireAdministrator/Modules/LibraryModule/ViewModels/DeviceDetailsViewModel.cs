using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace LibraryModule.ViewModels
{
    public class DeviceDetailsViewModel : SaveCancelDialogViewModel
	{
		public DeviceDetailsViewModel()
		{
			Title = "Добавить устройство";

            Devices = new List<DeviceViewModel>();
			foreach (var driver in FiresecManager.Drivers)
			{
                if (driver.IsPlaceable && !driver.IsIgnore && !FiresecManager.DeviceLibraryConfiguration.Devices.Any(x => x.DriverId == driver.UID))
                {
                    var libraryDevice = new LibraryDevice()
                    {
                        Driver = driver,
                        DriverId = driver.UID
                    };
                    var deviceViewModel = new DeviceViewModel(libraryDevice);
                    Devices.Add(deviceViewModel);
                }
			}
            SelectedDevice = Devices.FirstOrDefault();
		}

        public List<DeviceViewModel> Devices { get; private set; }
        public DeviceViewModel SelectedDevice { get; set; }

        protected override bool CanSave()
        {
            return SelectedDevice != null;
        }
	}
}