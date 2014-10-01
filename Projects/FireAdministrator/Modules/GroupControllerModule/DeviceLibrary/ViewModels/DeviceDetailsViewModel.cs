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
				if (!XManager.DeviceLibraryConfiguration.XDevices.Any(x => x.XDriverId == driver.UID) && (driver.IsPlaceable))
				{
					var libraryXDevice = new LibraryXDevice()
					{
						Driver = driver,
						XDriverId = driver.UID
					};
					var libraryXState = new LibraryXState()
					{
						XStateClass = XStateClass.No,
					};
					libraryXState.XFrames.Add(new LibraryXFrame() { Id = 0 });
					libraryXDevice.XStates.Add(libraryXState);

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