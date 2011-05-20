using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using System.Windows;
using DeviceLibrary;
using DeviceLibrary.Models;

namespace LibraryModule.ViewModels
{
    public class LibraryViewModel : RegionViewModel
    {
        public LibraryViewModel()
        {
            Current = this;
            SaveCommand = new RelayCommand(OnSave);
        }

        public static LibraryViewModel Current { get; private set; }
        public void Initialize()
        {
            Devices = new ObservableCollection<DeviceViewModel>();
            foreach (var device in LibraryManager.Devices)
            {
                var deviceViewModel = new DeviceViewModel();
                deviceViewModel.Id = device.Id;
                Devices.Add(deviceViewModel);
                foreach (var state in device.States)
                {
                    var stateViewModel = new StateViewModel();
                    stateViewModel.Initialize(state);
                    deviceViewModel.States.Add(stateViewModel);
                    foreach (var frame in state.Frames)
                    {
                        var frameViewModel = new FrameViewModel();
                        frameViewModel.Initialize(frame);
                        stateViewModel.Frames.Add(frameViewModel);
                    }
                }

                deviceViewModel.SortStates();
            }
        }

        private bool _flag;
        public bool Flag
        {
            get { return _flag; }
            set
            {
                _flag = value;
                OnPropertyChanged("Flag");
            }
        }

        private ObservableCollection<DeviceViewModel> _devices;
        public ObservableCollection<DeviceViewModel> Devices
        {
            get { return _devices; }
            set
            {
                _devices = value;
                OnPropertyChanged("Devices");
            }
        }

        private DeviceViewModel _selectedDevice;
        public DeviceViewModel SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                Flag = true;
                _selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
            }
        }

        private StateViewModel _selectedState;
        public StateViewModel SelectedState
        {
            get { return _selectedState; }
            set
            {
                Flag = true;
                _selectedState = value;
                if (value == null) return;
                var deviceControl = SelectedState.ParentDevice.DeviceControl;
                SelectedDevice = value.ParentDevice;
                SelectedState.SelectedFrame = value.Frames[0];
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
                    foreach (var stateId in SelectedState.ParentDevice.AdditionalStates)
                    {
                        var state = SelectedState.ParentDevice.States.FirstOrDefault(x => (x.Id == stateId) && (x.IsAdditional));
                        if (state.Class() == SelectedState.Id)
                            tempAstate.Add(state.Id);
                    }
                    deviceControl.AdditionalStates = tempAstate;
                }

                OnPropertyChanged("SelectedState");
            }
        }

        public RelayCommand SaveCommand { get; private set; }
        private void OnSave()
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
            if (!_flag) return;
            LibraryManager.Devices = new List<Device>();
            foreach (var deviceViewModel in Devices)
            {
                var device = new Device();
                device.Id = deviceViewModel.Id;
                LibraryManager.Devices.Add(device);
                device.States = new List<State>();
                foreach (var stateViewModel in deviceViewModel.States)
                {
                    var state = new State();
                    state.Id = stateViewModel.Id;
                    state.IsAdditional = stateViewModel.IsAdditional;
                    device.States.Add(state);
                    state.Frames = new List<Frame>();
                    foreach (var frameViewModel in stateViewModel.Frames)
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

            if (SelectedState == null) return;
            if (SelectedState.IsAdditional)
            {
                SelectedState.ParentDevice.DeviceControl.AdditionalStates = new List<string>() { SelectedState.Id };
                SelectedState.ParentDevice.DeviceControl.State = "-1";
            }
            else
            {
                SelectedState.ParentDevice.DeviceControl.State = SelectedState.Id;
            }
        }

        public override void Dispose()
        {
        }
    }
}
