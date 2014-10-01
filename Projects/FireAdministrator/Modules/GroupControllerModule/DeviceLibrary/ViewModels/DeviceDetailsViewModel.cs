using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DeviceDetailsViewModel : SaveCancelDialogViewModel
	{
		public DeviceDetailsViewModel()
		{
			Title = "Добавить устройство";

			Devices = new ObservableCollection<XDeviceViewModel>();
			var drivers = (from XDriver driver in XManager.Drivers select driver).ToList();
			foreach (var driver in drivers)
			{
				if (!XManager.DeviceLibraryConfiguration.GKDevices.Any(x => x.DriverUID == driver.UID) && (driver.IsPlaceable))
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

					var xdeviceViewModel = new XDeviceViewModel(gkLibraryDevice);
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