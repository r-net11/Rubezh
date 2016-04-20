using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevicesModule.Views;
using DevicesModule.ViewModels;
using Infrastructure.Common.Windows;
using CurrentDeviceModule.Views;

namespace CurrentDeviceModule.ViewModels
{
    public class CurrentDeviceDetailsViewModel : BaseViewModel
    {
        public CurrentDeviceDetailsViewModel(Guid deviceId)
        {
            DeviceDetailsViewModel = new DeviceDetailsViewModel(deviceId);
            ActiveXDeviceDetailsView = new ActiveXDeviceDetailsView();
            ActiveXDeviceDetailsView.DataContext = DeviceDetailsViewModel;
        }
        
        DeviceDetailsViewModel _deviceDetailsViewModel;
        public DeviceDetailsViewModel DeviceDetailsViewModel
        {
            get { return _deviceDetailsViewModel; }
            set
            {
                _deviceDetailsViewModel = value;
                OnPropertyChanged("DeviceDetailsViewModel");
            }
        }

        ActiveXDeviceDetailsView _activeXDeviceDetailsView;
        public ActiveXDeviceDetailsView ActiveXDeviceDetailsView
        {
            get { return _activeXDeviceDetailsView; }
            set
            {
                _activeXDeviceDetailsView = value;
                OnPropertyChanged("ActiveXDeviceDetailsView");
            }
        }
    }
}
