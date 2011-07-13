using System.Linq;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using DeviceLibrary;
using FiresecClient;

namespace LibraryModule.ViewModels
{
    internal class NewDeviceViewModel : DialogContent
    {
        public NewDeviceViewModel()
        {
            Title = "Добавить устройство";
            AddCommand = new RelayCommand(OnAdd);
            CancelCommand = new RelayCommand(OnCancel);
            Initialize();
        }

        public void Initialize()
        {
            Items = new ObservableCollection<DeviceViewModel>();
            foreach (var driver in FiresecManager.Configuration.Drivers)
            {
                if (!driver.IsPlaceable || (LibraryViewModel.Current.Devices.FirstOrDefault(x => x.Id == driver.Id) != null))
                    continue;

                var deviceViewModel = new DeviceViewModel();
                deviceViewModel.Id = driver.Id;
                deviceViewModel.Initialize();
                Items.Add(deviceViewModel);
            }
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
            var deviceViewModel = Items.FirstOrDefault(x => x.Id == SelectedItem.Id);
            LibraryViewModel.Current.Devices.Add(deviceViewModel);
            Items.Remove(_selectedItem);
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
