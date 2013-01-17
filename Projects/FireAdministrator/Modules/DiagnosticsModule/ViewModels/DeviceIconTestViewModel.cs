using System.Collections.ObjectModel;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace DiagnosticsModule.ViewModels
{
    public class DeviceIconTestViewModel : SaveCancelDialogViewModel
    {
        public DeviceIconTestViewModel()
        {
            Title = "";
            Drivers = new ObservableCollection<Driver>();
            foreach (Driver driver in FiresecManager.Drivers)
                Drivers.Add(driver);
            //Drivers = new List<Driver>(
            //    from Driver driver in FiresecManager.Drivers
            //    select driver);
            //Drivers = Drivers.OrderBy(driver => driver.ShortName).ToList();
        }

        public ObservableCollection<Driver> Drivers { get; private set; }
    }
}