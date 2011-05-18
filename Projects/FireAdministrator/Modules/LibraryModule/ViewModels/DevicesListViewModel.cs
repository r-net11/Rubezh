using System.Linq;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using DeviceLibrary;

namespace LibraryModule.ViewModels
{
    internal class DevicesListViewModel : DialogContent
    {
        private ObservableCollection<DeviceViewModel> _items;
        private DeviceViewModel _selectedItem;

        public DevicesListViewModel()
        {
            Title = "Список устройств";
            AddCommand = new RelayCommand(OnAdd);
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
    }
}
