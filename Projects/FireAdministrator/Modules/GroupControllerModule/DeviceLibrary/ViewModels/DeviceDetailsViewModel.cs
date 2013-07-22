using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class DeviceDetailsViewModel : SaveCancelDialogViewModel
	{
		public DeviceDetailsViewModel()
		{
			Title = "Добавить устройство";

			Devices = new ObservableCollection<XDeviceViewModel>();
			var drivers = (from XDriver driver in XManager.DriversConfiguration.XDrivers select driver).ToList();
			foreach (var driver in drivers)
			{
				if (!XManager.XDeviceLibraryConfiguration.XDevices.Any(x => x.XDriverId == driver.UID) && (driver.IsPlaceable))
				{
					var libraryXDevice = new LibraryXDevice()
					{
						Driver = driver,
						XDriverId = driver.UID
					};
					var xdeviceViewModel = new XDeviceViewModel(libraryXDevice);
					Devices.Add(xdeviceViewModel);
				}
			}
			SelectedDevice = Devices.FirstOrDefault();
		}

		public ObservableCollection<XDeviceViewModel> Devices { get; private set; }
		public XDeviceViewModel SelectedDevice { get; set; }

		protected override bool CanSave()
		{
			return SelectedDevice != null;
		}
	}
}