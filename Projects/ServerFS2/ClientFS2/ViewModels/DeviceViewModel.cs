using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using System.Linq;

namespace ClientFS2.ViewModels
{
    public class DeviceViewModel : TreeItemViewModel<DeviceViewModel>
    {
        public Device Device { get; private set; }
        public DeviceViewModel(Device device)
        {
            Device = device;
        }

        public string UsbChannel
        {
            get { return Device.Properties.FirstOrDefault(x => x.Name == "UsbChannel").Value; }
        }

        public string SerialNo
        {
            get { return Device.Properties.FirstOrDefault(x => x.Name == "SerialNo").Value; }
        }

        public string Version
        {
            get { return Device.Properties.FirstOrDefault(x => x.Name == "Version").Value; }
        }

        public string Address
        {
            get { return Device.PresentationAddress; }
            set
            {
                Device.SetAddress(value);
                if (Driver.IsChildAddressReservedRange)
                {
                    foreach (var deviceViewModel in Children)
                    {
                        deviceViewModel.OnPropertyChanged("Address");
                    }
                }
                OnPropertyChanged("Address");
            }
        }
        public bool IsUsed
        {
            get { return !Device.IsNotUsed; }
            set
            {
                FiresecManager.FiresecConfiguration.SetIsNotUsed(Device, !value);
                OnPropertyChanged("IsUsed");
                OnPropertyChanged("ShowOnPlan");
                OnPropertyChanged("PresentationZone");
                OnPropertyChanged("EditingPresentationZone");
            }
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
                }
            }
        }
    }
}
