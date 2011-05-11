using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Common;
using DeviceControls;
using DeviceLibrary.Models;
using Firesec;
using ListView = DeviceEditor.Views.ListView;
using System.Linq;

namespace DeviceEditor.ViewModels
{
    public class DeviceViewModel : BaseViewModel
    {
        public DeviceViewModel()
        {
            Current = this;
            DeviceControl = new DeviceControl();
            ShowDevicesCommand = new RelayCommand(OnShowDevices);
            RemoveDeviceCommand = new RelayCommand(OnRemoveDevice);
            ShowStatesCommand = new RelayCommand(OnShowStates);
            AdditionalStatesViewModel = new List<string>();
        }

        public static DeviceViewModel Current { get; private set; }

        public void LoadBaseStates()
        {
            var driver = LibraryManager.Drivers.FirstOrDefault(x => x.id == this.Id);
            this.StatesViewModel = new ObservableCollection<StateViewModel>();
            for (var stateId = 0; stateId < 9; stateId++)
            {
                if(stateId < 7)
                    if (driver.state.FirstOrDefault(x => x.@class == Convert.ToString(stateId)) == null) continue;
                var stateViewModel = new StateViewModel();
                stateViewModel.Id = Convert.ToString(stateId);
                var frameViewModel = new FrameViewModel();
                frameViewModel.Duration = 0;
                frameViewModel.Image = Helper.EmptyFrame;
                stateViewModel.FrameViewModels = new ObservableCollection<FrameViewModel>();
                stateViewModel.FrameViewModels.Add(frameViewModel);
                this.StatesViewModel.Add(stateViewModel);
                ViewModel.Current.Update();
            }
        }

        private DeviceControl _deviceControl;
        public DeviceControl DeviceControl
        {
            get { return _deviceControl; }
            set
            {
                _deviceControl = value;
                OnPropertyChanged("DeviceControl");
            }
        }

        private string _iconPath;
        public string IconPath
        {
            get { return _iconPath; }
            set
            {
                _iconPath = value;
                OnPropertyChanged("IconPath");
            }
        }

        public RelayCommand ShowDevicesCommand { get; private set; }
        private static void OnShowDevices(object obj)
        {
            var devicesListView = new ListView();
            var devicesListViewModel = new DevicesListViewModel();
            devicesListView.DataContext = devicesListViewModel;
            devicesListView.ShowDialog();
        }

        public RelayCommand ShowStatesCommand { get; private set; }
        private static void OnShowStates(object obj)
        {
            var statesListView = new ListView();
            var statesListViewModel = new StatesListViewModel();
            statesListView.DataContext = statesListViewModel;
            statesListView.ShowDialog();
        }

        public RelayCommand RemoveDeviceCommand { get; private set; }
        private void OnRemoveDevice(object obj)
        {
            ViewModel.Current.DeviceViewModels.Remove(this);
            ViewModel.Current.SelectedStateViewModel = null;
            ViewModel.Current.Update();
        }

        private string _id;
        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                Name = _name = DriversHelper.GetDriverNameById(_id);
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private StateViewModel _selectedStateViewModel;
        public StateViewModel SelectedStateViewModel
        {
            get { return _selectedStateViewModel; }
            set
            {
                if (value == null)
                    return;
                _selectedStateViewModel = value;
                ViewModel.Current.SelectedStateViewModel = _selectedStateViewModel;
                OnPropertyChanged("SelectedStateViewModel");
            }
        }

        private ObservableCollection<StateViewModel> _statesViewModel;
        public ObservableCollection<StateViewModel> StatesViewModel
        {
            get { return _statesViewModel; }
            set
            {
                _statesViewModel = value;
                OnPropertyChanged("StatesViewModel");
            }
        }

        public List<string> AdditionalStatesViewModel;

    }
}