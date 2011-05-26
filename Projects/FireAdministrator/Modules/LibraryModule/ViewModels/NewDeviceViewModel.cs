using System.Linq;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using DeviceLibrary;

namespace LibraryModule.ViewModels
{
    internal class NewDeviceViewModel : DialogContent
    {
        private ObservableCollection<DeviceViewModel> _items;
        private DeviceViewModel _selectedItem;

        public NewDeviceViewModel()
        {
            Title = "Добавить устройство";
            AddCommand = new RelayCommand(OnAdd);
            CancelCommand = new RelayCommand(OnCancel);
            Initialize();
        }

        public ObservableCollection<DeviceViewModel> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged("Items");
            }
        }

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

        public void Initialize()
        {
            Items = new ObservableCollection<DeviceViewModel>();
            foreach (var item in LibraryManager.Drivers)
                try
                {
                    if (!item.options.Contains("Placeable") ||
                        (LibraryViewModel.Current.Devices.FirstOrDefault(x => x.Id == item.id) != null)) continue;
                    var deviceViewModel = new DeviceViewModel();
                    deviceViewModel.Id = item.id;
                    deviceViewModel.Initialize();
                    Items.Add(deviceViewModel);
                }
                catch { }
        }

        public RelayCommand CancelCommand { get; private set; }
        private void OnCancel()
        {
            Close(false);
        }
    }
}
