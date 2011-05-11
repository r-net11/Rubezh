using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using DeviceLibrary.Models;

namespace DeviceEditor.ViewModels
{
    internal class StatesListViewModel : BaseViewModel
    {
        #region Private Fields
        private ObservableCollection<StateViewModel> _items;
        private StateViewModel _selectedItem;
        private string _title = "Список состояний";
        #endregion

        public StatesListViewModel()
        {
            Current = this;
            Load();
            AddCommand = new RelayCommand(OnAdd);
        }

        public static StatesListViewModel Current;

        public RelayCommand AddCommand { get; private set; }
        /// <summary>
        /// Заголовок окна - "Список состояний".
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

        public StateViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        public ObservableCollection<StateViewModel> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged("Items");
            }
        }

        public void Load()
        {
            Items = new ObservableCollection<StateViewModel>();
            var selectedDevice = ViewModel.Current.SelectedDeviceViewModel;
            var driver = LibraryManager.Drivers.FirstOrDefault(x => x.id == selectedDevice.Id);
            foreach (var item in driver.state)
                try
                {
                    if (selectedDevice.StatesViewModel.FirstOrDefault(x => (x.Id == item.id) && (x.IsAdditional)) != null) continue;
                    var stateViewModel = new StateViewModel();
                    stateViewModel.ParentDevice = ViewModel.Current.SelectedDeviceViewModel;
                    stateViewModel.IsAdditional = true;
                    stateViewModel.Id = item.id;
                    var frameViewModel = new FrameViewModel();
                    frameViewModel.Duration = 500;
                    frameViewModel.Layer = 0;
                    frameViewModel.Image = Helper.EmptyFrame;
                    stateViewModel.FrameViewModels = new ObservableCollection<FrameViewModel> { frameViewModel };
                    Items.Add(stateViewModel);
                }catch { }
        }

        private void OnAdd(object obj)
        {
            var stateViewModel = Items.FirstOrDefault(x => x.Id == SelectedItem.Id);
            ViewModel.Current.SelectedDeviceViewModel.StatesViewModel.Add(stateViewModel);
            Items.Remove(_selectedItem);
        }

    }
}