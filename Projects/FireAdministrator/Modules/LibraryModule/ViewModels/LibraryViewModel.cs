using System.Collections.ObjectModel;
using System.Windows;
using Common;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace LibraryModule.ViewModels
{
    public class LibraryViewModel : RegionViewModel
    {
        public LibraryViewModel()
        {
            DeviceViewModels = new ObservableCollection<DeviceViewModel>();
            if (FiresecManager.DeviceLibraryConfiguration.Devices.IsNotNullOrEmpty())
            {
                FiresecManager.DeviceLibraryConfiguration.Devices.ForEach(
                    device => DeviceViewModels.Add(new DeviceViewModel(device)));
            }

            AddDeviceCommand = new RelayCommand(OnAddDevice);
            RemoveDeviceCommand = new RelayCommand(OnRemoveDevice, CanRemoveDevice);
            SaveCommand = new RelayCommand(OnSave);
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
                FiresecManager.DeviceLibraryConfiguration.Devices.Add(addDeviceViewModel.SelectedItem.Device);
                DeviceViewModels.Add(addDeviceViewModel.SelectedItem);
                LibraryModule.HasChanges = true;
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
                FiresecManager.DeviceLibraryConfiguration.Devices.Remove(SelectedDeviceViewModel.Device);
                DeviceViewModels.Remove(SelectedDeviceViewModel);
                LibraryModule.HasChanges = true;
            }
        }

        bool CanRemoveDevice(object obj)
        {
            return SelectedDeviceViewModel != null;
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            var result = MessageBox.Show("Вы уверены что хотите сохранить все изменения на диск?",
                                         "Окно подтверждения", MessageBoxButton.OKCancel,
                                         MessageBoxImage.Question);

            if (result == MessageBoxResult.OK)
            {
                FiresecManager.SetDeviceLibraryConfiguration();
            }
        }

        public override void OnShow()
        {
            ServiceFactory.Layout.ShowMenu(new LibraryMenuViewModel(SaveCommand));
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}