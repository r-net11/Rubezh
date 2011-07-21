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
            List<DeviceViewModel> devicesList = new List<DeviceViewModel>();
            foreach (var device in LibraryManager.Devices)
            {
                devicesList.Add(new DeviceViewModel(this, device));
            }
            Devices = new ObservableCollection<DeviceViewModel>(
                from device in devicesList
                orderby device.Name
                select device);
        }

        ObservableCollection<DeviceViewModel> _devices;
        public ObservableCollection<DeviceViewModel> Devices
        {
            get
            {
                return _devices;
            }

            set
            {
                _devices = value;
                OnPropertyChanged("Devices");
            }
        }

        DeviceViewModel _selectedDevice;
        public DeviceViewModel SelectedDevice
        {
            get
            {
                return _selectedDevice;
            }

            set
            {
                _selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
            }
        }

        void Update()
        {
            LibraryManager.Devices = new List<Device>();
            foreach (var deviceViewModel in Devices)
            {
                var device = new Device();
                device.Id = deviceViewModel.Id;

                device.States = new List<State>();
                foreach (var stateViewModel in deviceViewModel.States)
                {
                    var state = new State();
                    state.Id = stateViewModel.Id;
                    state.Name = stateViewModel.Name;
                    state.IsAdditional = stateViewModel.IsAdditional;

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
                LibraryManager.Devices.Add(device);
            }
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            var result = MessageBox.Show("Вы уверены что хотите сохранить все изменения на диск?",
                                                      "Окно подтверждения", MessageBoxButton.OKCancel,
                                                      MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                Update();
                LibraryManager.Save();
            }
        }
    }
}
