using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common;

namespace LibraryModule.ViewModels
{
    public class NewDeviceViewModel : DialogContent
    {
        public NewDeviceViewModel()
        {
            Initialize();
        }

        void Initialize()
        {
            Title = "Добавить устройство";

            Items = new ObservableCollection<DeviceViewModel>();
            foreach (var driver in FiresecManager.Configuration.Drivers)
            {
                if (driver.IsPlaceable && LibraryViewModel.Current.Devices.Any(x => x.Id == driver.Id) == false)
                {
                    var deviceViewModel = new DeviceViewModel(driver.Id);
                    deviceViewModel.SetDefaultState();
                    Items.Add(deviceViewModel);
                }
            }

            AddCommand = new RelayCommand(OnAdd);
            CancelCommand = new RelayCommand(OnCancel);
        }

        private ObservableCollection<DeviceViewModel> _items;
        public ObservableCollection<DeviceViewModel> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged("Items");
            }
        }

        private DeviceViewModel _selectedItem;
        public DeviceViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        public RelayCommand AddCommand { get; private set; }
        private void OnAdd()
        {
            if (SelectedItem == null) return;
            LibraryViewModel.Current.Devices.Add(SelectedItem);
            //Items.Remove(SelectedItem);
            LibraryViewModel.Current.Update();
            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        private void OnCancel()
        {
            Close(false);
        }
    }
}
