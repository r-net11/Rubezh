using System.Linq;
using FiresecClient;

namespace LibraryModule.ViewModels
{
	public class DeviceDetailsViewModel : DetailsBaseViewModel<DeviceViewModel>
	{
		public DeviceDetailsViewModel()
			: base()
		{
			Title = "Добавить устройство";

			foreach (var driver in FiresecManager.Drivers)
			{
				if (driver.IsPlaceable && !driver.IsIgnore && !FiresecManager.LibraryConfiguration.Devices.Any(x => x.DriverId == driver.UID))
					Items.Add(new DeviceViewModel(DeviceViewModel.GetDefaultDriverWith(driver.UID)));
			}
		}
	}
}