using System.Collections.ObjectModel;
using System.Windows;
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
            if (FiresecManager.DeviceLibraryConfiguration.Devices == null ||
                FiresecManager.DeviceLibraryConfiguration.Devices.Count == 0)
                return;

            foreach (var device in FiresecManager.DeviceLibraryConfiguration.Devices)
            {
                DeviceViewModels.Add(new DeviceViewModel(device));
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
                //foreach (var dev in FiresecManager.DeviceLibraryConfiguration.Devices)
                //{
                //    var dev_ = new FiresecAPI.Models.DeviceLibrary.Device()
                //    {
                //        Id = dev.Id,
                //        States = new System.Collections.Generic.List<FiresecAPI.Models.DeviceLibrary.State>()
                //    };

                //    foreach (var state in dev.States)
                //    {
                //        var state_ = new FiresecAPI.Models.DeviceLibrary.State()
                //        {
                //            Code = state.Code,
                //            StateType = state.StateType,
                //            Frames = new System.Collections.Generic.List<FiresecAPI.Models.DeviceLibrary.Frame>()
                //        };

                //        foreach (var frame in state.Frames)
                //        {
                //            state_.Frames.Add(new FiresecAPI.Models.DeviceLibrary.Frame()
                //            {
                //                Id = frame.Id,
                //                Duration = frame.Duration,
                //                Layer = frame.Layer,
                //                Image = frame.Image
                //            });
                //        }

                //        dev_.States.Add(state_);
                //    }
                //    FiresecManager.DeviceLibraryConfiguration.Devices.Add(dev_);
                //}
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