using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class PasteAsViewModel : SaveCancelDialogContent
    {
        public PasteAsViewModel()
        {
            Title = "Выберите устройство";
            Drivers = new List<Driver>();
            foreach (var driver in FiresecManager.Drivers)
            {
                if (driver.Category == DeviceCategoryType.Device)
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