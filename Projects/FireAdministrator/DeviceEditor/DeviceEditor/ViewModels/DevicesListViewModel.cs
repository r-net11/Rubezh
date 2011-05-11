using System.Collections.ObjectModel;
using System.Linq;
using Common;
using DeviceLibrary.Models;
using Firesec.Metadata;

namespace DeviceEditor.ViewModels
{
    internal class DevicesListViewModel : BaseViewModel
    {
        private ObservableCollection<DeviceViewModel> _items;
        private DeviceViewModel _selectedItem;
        private string _title = "Список устройств";

        public DevicesListViewModel()
        {
            Load();
            AddCommand = new RelayCommand(OnAdd);
        }

        public RelayCommand AddCommand { get; private set; }
        /// <summary>
        /// Заголовок окна - "Список устройств".
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
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

        private void OnAdd(object obj)
        {
            var deviceViewModel = Items.FirstOrDefault(x => x.Id == SelectedItem.Id);
            ViewModel.Current.DeviceViewModels.Add(deviceViewModel);
            Items.Remove(_selectedItem);
        }

        public void Load()
        {
            Items = new ObservableCollection<DeviceViewModel>();
            foreach (var item in LibraryManager.Drivers)
                try
                {
                    if (!item.options.Contains("Placeable") ||
                        (ViewModel.Current.DeviceViewModels.FirstOrDefault(x => x.Id == item.id) != null)) continue;
                    var deviceViewModel = new DeviceViewModel();
                    deviceViewModel.Id = item.id;
                    deviceViewModel.IconPath = Helper.DevicesIconsPath + item.dev_icon + ".ico";
                    deviceViewModel.LoadBaseStates();
                    Items.Add(deviceViewModel);
                }catch{}
        }
    }
}