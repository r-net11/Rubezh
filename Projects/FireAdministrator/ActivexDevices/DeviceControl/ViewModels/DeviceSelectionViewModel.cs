using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using DeviceEditor;
using Common;

namespace DevicesControl
{
    class DeviceSelectionViewModel:BaseViewModel
    {
        public DeviceSelectionViewModel()
        {
            Current = this;
        }

        static public DeviceSelectionViewModel Current { get; private set; }
        ObservableCollection<DeviceViewModel> devicesViewModel;
        public ObservableCollection<DeviceViewModel> DevicesViewModel
        {
            get { return devicesViewModel; }
            set
            {
                devicesViewModel = value;
                OnPropertyChanged("DevicesViewModel");
            }
        }

        DeviceViewModel selectedDeviceViewModel;
        public DeviceViewModel SelectedDeviceViewModel
        {
            get { return selectedDeviceViewModel; }
            set
            {
                selectedDeviceViewModel = value;
                OnPropertyChanged("SelectedDeviceViewModel");
            }
        }

        StateViewModel selectedStateViewModel;
        public StateViewModel SelectedStateViewModel
        {
            get { return selectedStateViewModel; }
            set
            {
                selectedStateViewModel = value;
                ViewModel.Current.SelectedStateViewModel = selectedStateViewModel;
                OnPropertyChanged("SelectedStateViewModel");
            }
        }
    }
}
