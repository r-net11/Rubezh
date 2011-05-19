using System.Linq;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using DeviceLibrary;

namespace LibraryModule.ViewModels
{
    internal class StatesListViewModel : DialogContent
    {
        public StatesListViewModel()
        {
            Title = "Список состояний";
            Initialize();
            AddCommand = new RelayCommand(OnAdd);
        }

        private StateViewModel _selectedItem;
        public StateViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        private ObservableCollection<StateViewModel> _items;
        public ObservableCollection<StateViewModel> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged("Items");
            }
        }

        public void Initialize()
        {
            Items = new ObservableCollection<StateViewModel>();
            var selectedDevice = LibraryViewModel.Current.SelectedDevice;
            var driver = LibraryManager.Drivers.FirstOrDefault(x => x.id == selectedDevice.Id);
            foreach (var item in driver.state)
                try
                {
                    if (selectedDevice.States.FirstOrDefault(x => (x.Id == item.id) && (x.IsAdditional)) != null) continue;
                    var stateViewModel = new StateViewModel();
                    stateViewModel.ParentDevice = LibraryViewModel.Current.SelectedDevice;
                    stateViewModel.IsAdditional = true;
                    stateViewModel.Id = item.id;
                    var frameViewModel = new FrameViewModel();
                    frameViewModel.Duration = 300;
                    frameViewModel.Layer = 0;
                    frameViewModel.Image = Helper.EmptyFrame;
                    stateViewModel.Frames = new ObservableCollection<FrameViewModel> { frameViewModel };
                    Items.Add(stateViewModel);
                }
                catch { }
        }

        public RelayCommand AddCommand { get; private set; }
        private void OnAdd()
        {
            if (SelectedItem == null) return;
            var stateViewModel = Items.FirstOrDefault(x => x.Id == SelectedItem.Id);
            LibraryViewModel.Current.SelectedDevice.States.Add(stateViewModel);
            Items.Remove(_selectedItem);
            LibraryViewModel.Current.SelectedDevice.SortStates();
            LibraryViewModel.Current.Update();
        }
    }
}
