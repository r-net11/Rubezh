using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Common;
using DeviceLibrary;
using DeviceLibrary.Models;
using Frame = DeviceLibrary.Models.Frame;

namespace DeviceEditor.ViewModels
{
    public class ViewModel : BaseViewModel
    {
        public ViewModel()
        {
            Current = this;
            Load();
            SaveCommand = new RelayCommand(OnSave);
            _flag = 1;
        }

        private readonly int _flag;
        public static ViewModel Current { get; private set; }

        public void Load()
        {
            DeviceViewModels = new ObservableCollection<DeviceViewModel>();
            foreach (var device in LibraryManager.Devices)
            {
                var deviceViewModel = new DeviceViewModel();
                deviceViewModel.Id = device.Id;
                deviceViewModel.StatesViewModel = new ObservableCollection<StateViewModel>();
                DeviceViewModels.Add(deviceViewModel);
                try { deviceViewModel.IconPath = Helper.DevicesIconsPath + LibraryManager.Drivers.FirstOrDefault(x => x.id == deviceViewModel.Id).dev_icon + ".ico"; }
                catch { }
                foreach (var state in device.States)
                {
                    var stateViewModel = new StateViewModel();
                    stateViewModel.IsAdditional = state.IsAdditional;
                    stateViewModel.Id = state.Id;
                    deviceViewModel.StatesViewModel.Add(stateViewModel);
                    stateViewModel.FrameViewModels = new ObservableCollection<FrameViewModel>();
                    foreach (var frame in state.Frames)
                    {
                        var frameViewModel = new FrameViewModel();
                        frameViewModel.Id = frame.Id;
                        frameViewModel.Image = frame.Image;
                        frameViewModel.Duration = frame.Duration;
                        frameViewModel.Layer = frame.Layer;
                        stateViewModel.FrameViewModels.Add(frameViewModel);
                    }
                }
            }
        }

        public RelayCommand SaveCommand { get; private set; }
        private void OnSave(object obj)
        {
            var result = MessageBox.Show("Вы уверены что хотите сохранить все изменения на диск?",
                                                      "Окно подтверждения", MessageBoxButton.OKCancel,
                                                      MessageBoxImage.Question);
            if (result == MessageBoxResult.Cancel) return;
            Update();
            LibraryManager.Save();
        }

        public void Update()
        {
            if (_flag == 0) return;
            LibraryManager.Devices = new List<Device>();
            foreach (var deviceViewModel in DeviceViewModels)
            {
                var device = new Device();
                device.Id = deviceViewModel.Id;
                LibraryManager.Devices.Add(device);
                device.States = new List<State>();
                foreach (var stateViewModel in deviceViewModel.StatesViewModel)
                {
                    var state = new State();
                    state.Id = stateViewModel.Id;
                    state.IsAdditional = stateViewModel.IsAdditional;
                    device.States.Add(state);
                    state.Frames = new List<Frame>();
                    foreach (var frameViewModel in stateViewModel.FrameViewModels)
                    {
                        var frame = new Frame();
                        frame.Id = frameViewModel.Id;
                        frame.Image = frameViewModel.Image;
                        frame.Duration = frameViewModel.Duration;
                        frame.Layer = frameViewModel.Layer;
                        state.Frames.Add(frame);
                    }
                }
            }
            
            if (SelectedStateViewModel == null) return;
            if (SelectedStateViewModel.IsAdditional)
            {
                SelectedStateViewModel.ParentDevice.DeviceControl.AdditionalStates = new List<string>() { SelectedStateViewModel.Id };
                SelectedStateViewModel.ParentDevice.DeviceControl.State = "-1";
            }
            else
            {
                SelectedStateViewModel.ParentDevice.DeviceControl.State = SelectedStateViewModel.Id;
            }
        }

        private ObservableCollection<DeviceViewModel> _deviceViewModels;
        public ObservableCollection<DeviceViewModel> DeviceViewModels
        {
            get { return _deviceViewModels; }
            set
            {
                _deviceViewModels = value;
                OnPropertyChanged("DeviceViewModels");
            }
        }

        private DeviceViewModel _selectedDeviceViewModel;
        public DeviceViewModel SelectedDeviceViewModel
        {
            get { return _selectedDeviceViewModel; }
            set
            {
                _selectedDeviceViewModel = value;
                OnPropertyChanged("SelectedDeviceViewModel");
            }
        }

        private StateViewModel _selectedStateViewModel;
        public StateViewModel SelectedStateViewModel
        {
            get { return _selectedStateViewModel; }
            set
            {
                _selectedStateViewModel = value;
                if (value == null) return;
                var deviceControl = SelectedStateViewModel.ParentDevice.DeviceControl;
                SelectedDeviceViewModel = value.ParentDevice;
                SelectedStateViewModel.SelectedFrameViewModel = value.FrameViewModels[0];
                deviceControl.DriverId = value.ParentDevice.Id;

                if (value.IsAdditional)
                {
                    deviceControl.AdditionalStates = new List<string>() { value.Id };
                    deviceControl.State = "-1";
                }
                else
                {
                    deviceControl.State = value.Id;

                    var tempAstate = new List<string>();
                    foreach (var state in SelectedStateViewModel.ParentDevice.AdditionalStatesViewModel)
                    {
                        var astate = LibraryManager.Drivers.FirstOrDefault(x => x.id == SelectedDeviceViewModel.Id).state.FirstOrDefault(x=>x.id == state);
                        if(astate.@class == value.Id)
                            tempAstate.Add(astate.id);
                    }
                    deviceControl.AdditionalStates = tempAstate;
                }

                OnPropertyChanged("SelectedStateViewModel");
            }
        }
    }
}