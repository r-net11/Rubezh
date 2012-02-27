using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    [SaveSizeAttribute]
    public class NewDeviceViewModel : SaveCancelDialogContent
    {
        DeviceViewModel _parentDeviceViewModel;
        Device _parent;

        public NewDeviceViewModel(DeviceViewModel parent)
        {
            Title = "Новое устройство";
            _parentDeviceViewModel = parent;
            _parent = _parentDeviceViewModel.Device;
        }

        public IEnumerable<Driver> Drivers
        {
            get
            {
                return from Driver driver in FiresecManager.Drivers
                       where _parent.Driver.AvaliableChildren.Contains(driver.UID)
                       select driver;
            }
        }

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

        protected override bool CanSave()
        {
            return (SelectedDriver != null);
        }

        protected override void Save(ref bool cancel)
        {
            int address = NewDeviceHelper.GetMinAddress(SelectedDriver, _parent);
            Device device = _parent.AddChild(SelectedDriver, address);
            NewDeviceHelper.AddDevice(device, _parentDeviceViewModel);

            _parentDeviceViewModel.Update();
            FiresecManager.DeviceConfiguration.Update();
        }
    }
}