using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace ClientFS2.ViewModels
{
    public class DeviceViewModel : TreeItemViewModel<DeviceViewModel>
    {
        public DeviceViewModel(Device device, bool intitialize = true)
        {
            Device = device;
            if (!intitialize)
                return;
            PropertiesViewModel = new PropertiesViewModel(device);
            device.AUParametersChanged += device_AUParametersChanged;
        }
        public Device Device { get; private set; }
        public PropertiesViewModel PropertiesViewModel { get; private set; }
        public string Address
        {
            get { return Device.PresentationAddress; }
            set
            {
                Device.SetAddress(value);
                if (Driver.IsChildAddressReservedRange)
                {
                    foreach (DeviceViewModel deviceViewModel in Children)
                    {
                        deviceViewModel.OnPropertyChanged("Address");
                    }
                }
                OnPropertyChanged("Address");
            }
        }
        public string ConnectedTo
        {
            get { return Device.ConnectedTo; }
        }
        public Driver Driver
        {
            get { return Device.Driver; }
            set
            {
                if (Device.Driver.DriverType != value.DriverType)
                {
                    FiresecManager.FiresecConfiguration.ChangeDriver(Device, value);
                    OnPropertyChanged("Device");
                    OnPropertyChanged("Driver");
                    PropertiesViewModel = new PropertiesViewModel(Device);
                    OnPropertyChanged("PropertiesViewModel");
                }
            }
        }

        void device_AUParametersChanged()
        {
            UpdataConfigurationProperties();
            PropertiesViewModel.IsAuParametersReady = true;
        }

        public void UpdataConfigurationProperties()
        {
            PropertiesViewModel = new PropertiesViewModel(Device) { ParameterVis = true };
            OnPropertyChanged("PropertiesViewModel");
        }
    }
}