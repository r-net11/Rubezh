using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
    public class XDeviceDetailsViewModel : SaveCancelDialogViewModel
    {
        public XDeviceDetailsViewModel()
        {
            Title = "Добавить устройство";

            XDevices = new List<XDeviceViewModel>();
            XDrivers = (from XDriver driver in XManager.DriversConfiguration.XDrivers
                                  select driver).ToList();
            foreach (var driver in XDrivers)
            {
                if (!XManager.XDeviceLibraryConfiguration.XDevices.Any(x => x.XDriverId == driver.UID)&&(driver.CanPlaceOnPlan))
                {
                    var libraryXDevice = new LibraryXDevice()
                    {
                        XDriver = driver,
                        XDriverId = driver.UID
                    };
                    var xdeviceViewModel = new XDeviceViewModel(libraryXDevice);
                    XDevices.Add(xdeviceViewModel);
                }
            }
            SelectedXDevice = XDevices.FirstOrDefault();
        }

        public List<XDeviceViewModel> XDevices { get; private set; }
        public List<XDriver> XDrivers { get; private set; }

        public XDeviceViewModel SelectedXDevice { get; set; }

        protected override bool CanSave()
        {
            return SelectedXDevice != null;
        }
    }
}