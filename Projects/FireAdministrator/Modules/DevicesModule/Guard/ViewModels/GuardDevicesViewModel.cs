using System.Collections.ObjectModel;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class GuardDevicesViewModel : RegionViewModel
    {
        public GuardDevicesViewModel()
        {
            ShowSynchronizationCommand = new RelayCommand(OnShowSynchronization, CanShowSynchronization);
            Devices = new ObservableCollection<Device>();

            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                if ((device.Driver.DriverType == DriverType.USB_Rubezh_2OP) || (device.Driver.DriverType == DriverType.Rubezh_2OP))
                    Devices.Add(device);
            }
        }

        public ObservableCollection<Device> Devices { get; private set; }

        Device _selectedDevice;
        public Device SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
            }
        }

        public bool CanShowSynchronization()
        {
            return SelectedDevice != null;
        }

        public RelayCommand ShowSynchronizationCommand { get; private set; }
        void OnShowSynchronization()
        {
            var guardSynchronizationViewModel = new GuardSynchronizationViewModel(SelectedDevice);
            if (ServiceFactory.UserDialogs.ShowModalWindow(guardSynchronizationViewModel))
            {
                ServiceFactory.SaveService.DevicesChanged = true;
            }
        }

        public override void OnShow()
        {
            ServiceFactory.Layout.ShowMenu(new GuardDevicesMenuViewModel(this));
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}
