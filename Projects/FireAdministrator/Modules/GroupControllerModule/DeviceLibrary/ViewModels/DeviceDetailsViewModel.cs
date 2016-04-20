using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class DeviceDetailsViewModel : SaveCancelDialogViewModel
	{
		public DeviceDetailsViewModel()
		{
			Title = "Добавить устройство";

			Devices = new ObservableCollection<LibraryDeviceViewModel>();
			var drivers = (from GKDriver driver in GKManager.Drivers select driver).ToList();
			foreach (var driver in drivers)
			{
				if (!GKManager.DeviceLibraryConfiguration.GKDevices.Any(x => x.DriverUID == driver.UID) && (driver.IsPlaceable))
				{
					var gkLibraryDevice = new GKLibraryDevice()
					{
						Driver = driver,
						DriverUID = driver.UID
					};
					var libraryState = new GKLibraryState()
					{
						StateClass = XStateClass.No,
					};
					libraryState.Frames.Add(new GKLibraryFrame() { Id = 0 });
					gkLibraryDevice.States.Add(libraryState);

					var deviceViewModel = new LibraryDeviceViewModel(gkLibraryDevice);
					Devices.Add(deviceViewModel);
				}
			}
			SelectedDevice = Devices.FirstOrDefault();
		}

		public ObservableCollection<LibraryDeviceViewModel> Devices { get; private set; }
		public LibraryDeviceViewModel SelectedDevice { get; set; }

		protected override bool CanSave()
		{
			return SelectedDevice != null;
		}
	}
}