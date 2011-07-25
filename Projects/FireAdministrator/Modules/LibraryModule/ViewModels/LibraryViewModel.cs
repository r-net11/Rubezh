using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DeviceLibrary;
using DeviceLibrary.Models;
using Infrastructure.Common;

namespace LibraryModule.ViewModels
{
    public class LibraryViewModel : RegionViewModel
    {
        public LibraryViewModel()
        {
            SaveCommand = new RelayCommand(OnSave);
        }

        public void Initialize()
        {
            var devicesList = new ObservableCollection<DeviceViewModel>();
            foreach (var device in LibraryManager.Devices)
            {
                devicesList.Add(new DeviceViewModel(this, device));
            }
            Devices = devicesList;
        }

        ObservableCollection<DeviceViewModel> _devices;
        public ObservableCollection<DeviceViewModel> Devices
        {
            get { return _devices; }

            private set
            {
                _devices = value;
                OnPropertyChanged("Devices");
            }
        }

        DeviceViewModel _selectedDevice;
        public DeviceViewModel SelectedDevice
        {
            get { return _selectedDevice; }

            set
            {
                _selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
            }
        }

        void UpdateModel()
        {
            var devices = new List<Device>();
            foreach (var deviceViewModel in Devices)
            {
                if (!deviceViewModel.Driver.IsIgnore && deviceViewModel.Driver.IsPlaceable)
                {
                    var device = new Device();
                    device.Id = deviceViewModel.Id;

                    device.States = new List<State>();
                    foreach (var stateViewModel in deviceViewModel.States)
                    {
                        var state = new State();
                        state.Class = stateViewModel.Class;
                        state.Code = stateViewModel.Code;

                        state.Frames = new List<Frame>();
                        foreach (var frameViewModel in stateViewModel.Frames)
                        {
                            var frame = new Frame();
                            frame.Id = frameViewModel.Id;
                            frame.Image = frameViewModel.XmlOfImage;
                            frame.Duration = frameViewModel.Duration;
                            frame.Layer = frameViewModel.Layer;

                            state.Frames.Add(frame);
                        }
                        device.States.Add(state);
                    }
                    devices.Add(device);
                }
            }
            LibraryManager.Devices = devices;
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            var result = MessageBox.Show("Вы уверены что хотите сохранить все изменения на диск?",
                                         "Окно подтверждения", MessageBoxButton.OKCancel,
                                         MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                UpdateModel();
                LibraryManager.Save();
            }
        }
    }
}
