using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class PasteAsViewModel : SaveCancelDialogContent
    {
        public PasteAsViewModel(DriverType parentDriverType)
        {
            Title = "Выберите устройство";
            Drivers = new List<Driver>();
            var driverTypes = new List<DriverType>();

            switch (parentDriverType)
            {
                case DriverType.Computer:
                    driverTypes = DriversHelper.UsbPanelDrivers;
                    break;
                case DriverType.USB_Channel_1:
                case DriverType.USB_Channel_2:
                    driverTypes = DriversHelper.PanelDrivers;
                    break;
            }

            foreach (var driver in FiresecManager.Drivers)
            {
                if (driverTypes.Contains(driver.DriverType))
                    Drivers.Add(driver);
            }
        }

        public List<Driver> Drivers { get; private set; }

        Driver _selectedDriver;
        public Driver SelectedDriver
        {
            get { return _selectedDriver; }
            set
            {
                _selectedDriver = value;
                OnPropertyChanged("SelectedDriver");
            }
        }
    }
}