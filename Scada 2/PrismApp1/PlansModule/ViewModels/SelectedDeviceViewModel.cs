using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using Infrastructure.Events;
using ClientApi;

namespace PlansModule.ViewModels
{
    public class SelectedDeviceViewModel : BaseViewModel
    {
        public SelectedDeviceViewModel()
        {
            ShowCommand = new RelayCommand(OnShow);
        }

        Device device;

        public void Initialize(string path)
        {
            if (ServiceClient.CurrentConfiguration.AllDevices.Any(x => x.Path == path))
            {
                device = ServiceClient.CurrentConfiguration.AllDevices.FirstOrDefault(x => x.Path == path);
                Firesec.Metadata.drvType Driver = ServiceClient.CurrentConfiguration.Metadata.drv.FirstOrDefault(x => x.id == device.DriverId);
                Name = Driver.shortName;
            }
        }

        public RelayCommand ShowCommand { get; private set; }
        void OnShow()
        {
            //string path = @"F8340ECE-C950-498D-88CD-DCBABBC604F3:Компьютер/FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6:0/780DE2E6-8EDD-4CFA-8320-E832EB699544:1/B476541B-5298-4B3E-A9BA-605B839B1011:6/1E045AD6-66F9-4F0B-901C-68C46C89E8DA:1.62";
            ServiceFactory.Events.GetEvent<ShowDevicesEvent>().Publish(device.Path);
        }

        string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                OnPropertyChanged("IsActive");
            }
        }
    }
}
