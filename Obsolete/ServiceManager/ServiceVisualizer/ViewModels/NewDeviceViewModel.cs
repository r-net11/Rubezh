using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Collections.ObjectModel;
using System.Windows;
using FiresecClient;

namespace ServiceVisualizer
{
    public class NewDeviceViewModel : BaseViewModel
    {
        public NewDeviceViewModel()
        {
            AddCommand = new RelayCommand(OnAddCommand);
        }

        public event Action RequestClose;
        void OnRequestClose()
        {
            if (RequestClose != null)
                RequestClose();
        }

        DeviceViewModel ParentDeviceViewModel;

        public void Init(DeviceViewModel deviceViewModel)
        {
            ParentDeviceViewModel = deviceViewModel;
            AvailableDevices = new ObservableCollection<AvailableDevice>();

            foreach (Firesec.Metadata.drvType childDriver in FiresecManager.CurrentConfiguration.Metadata.drv)
            {
                Firesec.Metadata.classType childClass = FiresecManager.CurrentConfiguration.Metadata.@class.FirstOrDefault(x => x.clsid == childDriver.clsid);
                if ((childClass.parent != null) && (childClass.parent.Any(x => x.clsid == deviceViewModel.Driver.clsid)))
                {
                    if ((childDriver.lim_parent != null) && (childDriver.lim_parent != deviceViewModel.Driver.id))
                        continue;
                    if (childDriver.acr_enabled == "1")
                        continue;
                    AvailableDevice availableDevice = new AvailableDevice();
                    availableDevice.Init(childDriver);
                    AvailableDevices.Add(availableDevice);
                }
            }
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAddCommand(object obj)
        {
            OnRequestClose();
        }

        ObservableCollection<AvailableDevice> availableDevices;
        public ObservableCollection<AvailableDevice> AvailableDevices
        {
            get { return availableDevices; }
            set
            {
                availableDevices = value;
                OnPropertyChanged("AvailableDevices");
            }
        }

        AvailableDevice selectedAvailableDevice;
        public AvailableDevice SelectedAvailableDevice
        {
            get { return selectedAvailableDevice; }
            set
            {
                selectedAvailableDevice = value;
                OnPropertyChanged("SelectedAvailableDevice");
            }
        }

        bool closeSignal = false;
        bool CloseSignal
        {
            get { return closeSignal; }
            set
            {
                closeSignal = value;
                OnPropertyChanged("CloseSignal");
            }
        }
    }

    public class AvailableDevice
    {
        public void Init(Firesec.Metadata.drvType driver)
        {
            DriverId = driver.id;
            DriverName = driver.shortName;

            string ImageName;
            if (!string.IsNullOrEmpty(driver.dev_icon))
            {
                ImageName = driver.dev_icon;
            }
            else
            {
                Firesec.Metadata.classType metadataClass = FiresecManager.CurrentConfiguration.Metadata.@class.FirstOrDefault(x => x.clsid == driver.clsid);
                ImageName = metadataClass.param.FirstOrDefault(x => x.name == "Icon").value;
            }

            ImageSource = @"C:\Program Files\Firesec\Icons\" + ImageName + ".ico";
        }

        public string DriverId { get; set; }
        public string DriverName { get; set; }
        public string ImageSource { get; set; }
    }
}
