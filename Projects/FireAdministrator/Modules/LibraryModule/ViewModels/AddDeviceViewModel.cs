using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common;

namespace LibraryModule.ViewModels
{
    public class AddDeviceViewModel : DialogContent
    {
        public AddDeviceViewModel(LibraryViewModel parentLibrary)
        {
            ParentLibrary = parentLibrary;
            Initialize();
        }

        void Initialize()
        {
            Title = "Добавить устройство";

            Items = new ObservableCollection<DeviceViewModel>();
            foreach (var driver in FiresecManager.Configuration.Drivers)
            {
                if (driver.IsPlaceable && !ParentLibrary.Devices.Any(x => x.Id == driver.Id))
                {
                    var deviceViewModel = new DeviceViewModel(ParentLibrary, driver);
                    Items.Add(deviceViewModel);
                }
            }
            Items = new ObservableCollection<DeviceViewModel>(
                 from item in Items
                 orderby item.Name
                 select item);

            OkCommand = new RelayCommand(OnOk);
            CancelCommand = new RelayCommand(OnCancel);
        }

        LibraryViewModel ParentLibrary { get; set; }

        ObservableCollection<DeviceViewModel> _items;
        public ObservableCollection<DeviceViewModel> Items
        {
            get
            {
                return _items;
            }

            set
            {
                _items = value;
                OnPropertyChanged("Items");
            }
        }

        DeviceViewModel _selectedItem;
        public DeviceViewModel SelectedItem
        {
            get
            {
                return _selectedItem;
            }

            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        public RelayCommand OkCommand { get; private set; }
        void OnOk()
        {
            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}
