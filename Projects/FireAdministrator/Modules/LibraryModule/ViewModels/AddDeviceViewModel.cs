using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;

namespace LibraryModule.ViewModels
{
    public class AddDeviceViewModel : AddViewModel<LibraryViewModel, DeviceViewModel>
    {
        public AddDeviceViewModel(LibraryViewModel parentLibrary)
            : base(parentLibrary) { }

        override public void Initialize()
        {
            Title = "Добавить устройство";

            var items = new ObservableCollection<DeviceViewModel>();
            foreach (var driver in FiresecManager.Configuration.Drivers)
            {
                if (driver.IsPlaceable && !Parent.Devices.Any(x => x.Id == driver.Id))
                {
                    items.Add(new DeviceViewModel(Parent, driver));
                }
            }
            Items = new ObservableCollection<DeviceViewModel>(
                        from item in items
                        orderby item.Name
                        select item);
        }
    }
}
