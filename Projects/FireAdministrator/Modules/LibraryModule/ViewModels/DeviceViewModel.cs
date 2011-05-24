using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using DeviceControls;
using DeviceLibrary;
using Firesec;
using Infrastructure;

namespace LibraryModule.ViewModels
{
    public class DeviceViewModel : BaseViewModel
    {
        public DeviceViewModel()
        {
            Current = this;
            DeviceControl = new DeviceControl();
            ShowDevicesCommand = new RelayCommand(OnShowDevices);
            RemoveDeviceCommand = new RelayCommand(OnRemoveDevice);
            ShowAdditionalStatesCommand = new RelayCommand(OnShowAdditionalStates);
            ShowStatesCommand = new RelayCommand(OnShowStates);
            AdditionalStates = new List<string>();
            States = new ObservableCollection<StateViewModel>();
        }

        public static DeviceViewModel Current { get; private set; }

        public void Initialize()
        {
            var driver = LibraryManager.Drivers.FirstOrDefault(x => x.id == Id);
            States = new ObservableCollection<StateViewModel>();
            var stateViewModel = new StateViewModel();
            stateViewModel.Id = Convert.ToString(8);
            var frameViewModel = new FrameViewModel(Helper.EmptyFrame, 300, 0);
            stateViewModel.Frames = new ObservableCollection<FrameViewModel>() { frameViewModel };
            States = new ObservableCollection<StateViewModel>(){stateViewModel};
            LibraryViewModel.Current.Update();
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

        public void SortStates()
        {
            var sortedStates = new ObservableCollection<StateViewModel>(States);
            foreach (var state in States)
            {
                if (!state.IsAdditional) continue;
                sortedStates.Remove(state);
                var index = sortedStates.IndexOf(sortedStates.FirstOrDefault(x => (x.Id == state.Class) && (!x.IsAdditional)));
                sortedStates.Insert(index+1, state);
            }
            States = sortedStates;
        }

        public string IconPath
        {
            get
            {
                try
                {
                    return Helper.DevicesIconsPath + LibraryManager.Drivers.FirstOrDefault(x => x.id == Id).dev_icon + ".ico";
                }
                catch (Exception)
                {
                }
                return null;
            }
        }

        private string  _id;
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

        private StateViewModel _selectedState;
        public StateViewModel SelectedState
        {
            get { return _selectedState; }
            set
            {
                if (value == null)
                    return;
                _selectedState = value;
                LibraryViewModel.Current.SelectedState = _selectedState;
                OnPropertyChanged("SelectedState");
            }
        }

        private ObservableCollection<StateViewModel> _states;
        public ObservableCollection<StateViewModel> States
        {
            get { return _states; }
            set
            {
                _states = value;
                OnPropertyChanged("States");
            }
        }

        public List<string> AdditionalStates;

        public RelayCommand ShowDevicesCommand { get; private set; }
        private static void OnShowDevices()
        {
            var devicesListViewModel = new DevicesListViewModel();
            ServiceFactory.UserDialogs.ShowModalWindow(devicesListViewModel);
        }

        public RelayCommand ShowStatesCommand { get; private set; }
        public static void OnShowStates()
        {
            var statesListViewModel = new StatesListViewModel();
            ServiceFactory.UserDialogs.ShowModalWindow(statesListViewModel);
        }

        public RelayCommand ShowAdditionalStatesCommand { get; private set; }
        public static void OnShowAdditionalStates()
        {
            var additionalStatesListViewModel = new AdditionalStatesListViewModel();
            ServiceFactory.UserDialogs.ShowModalWindow(additionalStatesListViewModel);
        }

        public RelayCommand RemoveDeviceCommand { get; private set; }
        private void OnRemoveDevice()
        {
            LibraryViewModel.Current.Devices.Remove(this);
            LibraryViewModel.Current.SelectedState = null;
            LibraryViewModel.Current.Update();
        }
    }
}
