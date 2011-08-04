using System.Collections.ObjectModel;
using System.Windows;
using DeviceLibrary;
using Infrastructure;
using Infrastructure.Common;

namespace LibraryModule.ViewModels
{
    public class LibraryViewModel : RegionViewModel
    {
        public LibraryViewModel() { }

        public void Initialize()
        {
            AddDeviceCommand = new RelayCommand(OnAddDevice);
            RemoveDeviceCommand = new RelayCommand(OnRemoveDevice);
            SaveCommand = new RelayCommand(OnSave);
        }

        public ObservableCollection<DeviceViewModel> DeviceViewModels
        {
            get
            {
                var deviceViewModels = new ObservableCollection<DeviceViewModel>();
                foreach (var device in LibraryManager.Devices)
                {
                    deviceViewModels.Add(new DeviceViewModel(device));
                }

                return deviceViewModels;
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

        public RelayCommand AddDeviceCommand { get; private set; }
        void OnAddDevice()
        {
            var addDeviceViewModel = new DeviceDetailsViewModel();
            if (ServiceFactory.UserDialogs.ShowModalWindow(addDeviceViewModel))
            {
                LibraryManager.Devices.Add(addDeviceViewModel.SelectedItem.Device);
                OnPropertyChanged("DeviceViewModels");
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
                LibraryManager.Devices.Remove(SelectedDeviceViewModel.Device);
                OnPropertyChanged("DeviceViewModels");
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
                LibraryManager.Save();
            }
        }
    }
}
