using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
    public class NewDeviceViewModel : SaveCancelDialogViewModel
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

		protected override bool Save()
		{
            byte address = NewDeviceHelper.GetMinAddress(SelectedDriver, _parent);
            XDevice xDevice = _parent.AddChild(SelectedDriver, 1, address);
            NewDeviceHelper.AddDevice(xDevice, _parentDeviceViewModel);

            _parentDeviceViewModel.Update();
            XManager.DeviceConfiguration.Update();
			return base.Save();
		}
    }
}