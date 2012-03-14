using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Controls.MessageBox;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace LibraryModule.ViewModels
{
    public class LibraryViewModel : RegionViewModel
    {
        public LibraryViewModel()
        {
            AddDeviceCommand = new RelayCommand(OnAddDevice);
            RemoveDeviceCommand = new RelayCommand(OnRemoveDevice, CanRemoveDevice);

            // Чтобы протестить случай, когда для какого-то устройства из библиотеки нету драйвера.
            //FiresecManager.LibraryConfiguration.Devices[0].DriverId = System.Guid.NewGuid();
            DeviceViewModels = new ObservableCollection<DeviceViewModel>(
                FiresecManager.LibraryConfiguration.Devices.Select(device => new DeviceViewModel(device))
            );

            if (DeviceViewModels.Count > 0)
                SelectedDeviceViewModel = DeviceViewModels[0];
        }

        public ObservableCollection<DeviceViewModel> DeviceViewModels { get; private set; }

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
                FiresecManager.LibraryConfiguration.Devices.Add(addDeviceViewModel.SelectedItem.LibraryDevice);
                DeviceViewModels.Add(addDeviceViewModel.SelectedItem);

                ServiceFactory.SaveService.LibraryChanged = true;
            }
        }

        public RelayCommand RemoveDeviceCommand { get; private set; }
        void OnRemoveDevice()
        {
            var result = MessageBoxService.ShowQuestion("Вы уверены что хотите удалить выбранное устройство?");

            if (result == MessageBoxResult.Yes)
            {
                FiresecManager.LibraryConfiguration.Devices.Remove(SelectedDeviceViewModel.LibraryDevice);
                DeviceViewModels.Remove(SelectedDeviceViewModel);

                ServiceFactory.SaveService.LibraryChanged = true;
            }
        }

        bool CanRemoveDevice()
        {
            return SelectedDeviceViewModel != null;
        }
    }
}