using System.Collections.ObjectModel;
using System.Windows;
using FiresecAPI.Models;
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
            SaveCommand = new RelayCommand(OnSave);
        }

        public void Initialize()
        {
            DeviceViewModels = new ObservableCollection<DeviceViewModel>();
            if (FiresecManager.DeviceLibraryConfiguration.Devices.IsNotNullOrEmpry())
            {
                FiresecManager.DeviceLibraryConfiguration.Devices.ForEach(
                    device => DeviceViewModels.Add(new DeviceViewModel(device)));
            }
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