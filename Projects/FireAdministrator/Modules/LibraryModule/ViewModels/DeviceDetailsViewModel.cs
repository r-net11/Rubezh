using System.Linq;
using FiresecClient;

namespace LibraryModule.ViewModels
{
    public class DeviceDetailsViewModel : DetailsBaseViewModel<DeviceViewModel>
    {
        public DeviceDetailsViewModel()
        {
            Initialize("Добавить устройство");

            foreach (var driver in FiresecManager.Drivers)
            {
                if (driver.IsPlaceable &&
                    !driver.IsIgnore &&
                    !FiresecManager.DeviceLibraryConfiguration.Devices.Any(x => x.Id == driver.Id))
                {
                    Items.Add(new DeviceViewModel(DeviceViewModel.GetDefaultDriverWith(driver.Id)));
                }
            }
        }
    }
}