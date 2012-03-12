using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using GroupControllerModule.Models;
using System.Collections.ObjectModel;
using Controls.MessageBox;

namespace GroupControllerModule.ViewModels
{
    public class GCDeviceViewModel : TreeBaseViewModel<GCDeviceViewModel>
    {
        public GCDevice Device { get; private set; }

        public GCDeviceViewModel(GCDevice gCdevice, ObservableCollection<GCDeviceViewModel> sourceDevices)
        {
            Device = gCdevice;
            Source = sourceDevices;
        }

        public GCDriver Driver
        {
            get { return Device.Driver; }
            set
            {
                Device.Driver = value;
            }
        }

        public string Address
        {
            get { return Device.Address; }
            set
            {
                if (Device.Parent.Children.Where(x => x != Device).Any(x => x.Address == value))
                {
                    MessageBoxService.Show("Устройство с таким адресом уже существует");
                }
                else
                {
                    Device.Address = value;
                    if (Driver.IsChildAddressReservedRange)
                    {
                        foreach (var deviceViewModel in Children)
                        {
                            deviceViewModel.OnPropertyChanged("Address");
                        }
                    }
                }
                OnPropertyChanged("Address");
            }
        }

        public bool CanEditAddress
        {
            get
            {
                if (Parent != null && Parent.Driver.IsChildAddressReservedRange && Parent.Driver.DriverType != GCDriverType.MRK_30)
                    return false;
                return Driver.CanEditAddress;
            }
        }

        public string Description
        {
            get { return Device.Description; }
            set
            {
                Device.Description = value;
                OnPropertyChanged("Description");
            }
        }
    }
}