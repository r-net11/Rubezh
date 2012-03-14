using System.Collections.Generic;
using System.Linq;
using GroupControllerModule.Models;
using Infrastructure.Common;

namespace GroupControllerModule.ViewModels
{
    public class NewDeviceViewModel : SaveCancelDialogContent
    {
        DeviceViewModel _parentDeviceViewModel;
        XDevice _parent;

        public NewDeviceViewModel(DeviceViewModel parent)
        {
            Title = "Новое устройство";
            _parentDeviceViewModel = parent;
            _parent = _parentDeviceViewModel.Device;
        }

        public IEnumerable<XDriver> Drivers
        {
            get
            {
                return from XDriver xDriver in XManager.DriversConfiguration.Drivers
                       where _parent.Driver.Children.Contains(xDriver.UID)
                       select xDriver;
            }
        }

        XDriver _selectedDriver;
        public XDriver SelectedDriver
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
            XDevice xDevice = _parent.AddChild(SelectedDriver, address);
            NewDeviceHelper.AddDevice(xDevice, _parentDeviceViewModel);

            _parentDeviceViewModel.Update();
            XManager.DeviceConfiguration.Update();
        }
    }
}