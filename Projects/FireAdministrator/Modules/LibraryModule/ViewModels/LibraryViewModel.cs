using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using DeviceLibrary;
using Infrastructure;
using Infrastructure.Common;

namespace LibraryModule.ViewModels
{
    public class LibraryViewModel : RegionViewModel
    {
        public LibraryViewModel()
        {
            AddDeviceCommand = new RelayCommand(OnAddDevice);
            RemoveDeviceCommand = new RelayCommand(OnRemoveDevice);
            SaveCommand = new RelayCommand(OnSave);
        }

        public void Initialize()
        {
            DeviceViewModels = new ObservableCollection<DeviceViewModel>();
            foreach (var device in LibraryManager.Devices)
            {
                DeviceViewModels.Add(new DeviceViewModel(device));
            }
        }
        ObservableCollection<DeviceViewModel> _deviceViewModels;

        public ObservableCollection<DeviceViewModel> DeviceViewModels
        {
            get { return _deviceViewModels; }
            set
            {
                _deviceViewModels = value;
                OnPropertyChanged("DeviceViewModels");
            }
        }

        DeviceViewModel _selectedDeviceViewModel;
        public DeviceViewModel SelectedDeviceViewModel
        {
            get { return _selectedDeviceViewModel; }
            set
            {
                _selectedDeviceViewModel = value;
                OnPropertyChanged("SelectedDeviceViewModel");
            }
        }

        public List<DeviceLibrary.Models.Device> GetModel()
        {
            var devices = new List<DeviceLibrary.Models.Device>();
            foreach (var deviceViewModel in DeviceViewModels)
            {
                devices.Add(deviceViewModel.GetModel());
            }

            return devices;
        }

        public RelayCommand AddDeviceCommand { get; private set; }
        void OnAddDevice()
        {
            var addDeviceViewModel = new DeviceDetailsViewModel();
            if (ServiceFactory.UserDialogs.ShowModalWindow(addDeviceViewModel))
            {
                DeviceViewModels.Add(addDeviceViewModel.SelectedItem);
            }
        }

        public RelayCommand RemoveDeviceCommand { get; private set; }
        void OnRemoveDevice()
        {
            var result = MessageBox.Show("Вы уверены что хотите удалить выбранное устройство?",
                                          "Окно подтверждения",
                                          MessageBoxButton.OKCancel,
                                          MessageBoxImage.Question);

            if (result == MessageBoxResult.OK)
            {
                DeviceViewModels.Remove(SelectedDeviceViewModel);
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
                LibraryManager.Devices = GetModel();
                LibraryManager.Save();
            }
        }

        public override void OnShow()
        {
            LibraryMenuViewModel libraryMenuViewModel = new LibraryMenuViewModel(SaveCommand);
            ServiceFactory.Layout.ShowMenu(libraryMenuViewModel);
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}
