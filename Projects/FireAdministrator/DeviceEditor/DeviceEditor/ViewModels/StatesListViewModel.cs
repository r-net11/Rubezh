using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Common;
using DeviceLibrary;
using Firesec.Metadata;

namespace DeviceEditor.ViewModels
{
    internal class StatesListViewModel : BaseViewModel
    {
        private ObservableCollection<StateViewModel> _items;
        private StateViewModel _selectedItem;
        private string _title = "Список состояний";
        public StatesListViewModel()
        {
            Current = this;
            Load();
            AddCommand = new RelayCommand(OnAddCommand);
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
        private void OnAddCommand(object obj)
        {
            var stateViewModel = new
            StateViewModel
            {
                Id = _selectedItem.Id,
                IsAdditional = _selectedItem.IsAdditional,
                ParentDevice = ViewModel.Current.SelectedDeviceViewModel
            };
            var frameViewModel = new
            FrameViewModel
            {
                Duration = 500,
                Image = Helper.EmptyFrame
            };
            stateViewModel.FrameViewModels = new
            ObservableCollection<FrameViewModel> {frameViewModel};
            ViewModel.Current.SelectedDeviceViewModel.StatesViewModel.Add(stateViewModel);
            Items.Remove(_selectedItem);
        }
        public void Load()
        {
            Items = new ObservableCollection<StateViewModel>();
            var driver = LibraryManager.Drivers.FirstOrDefault(x => x.id == ViewModel.Current.SelectedDeviceViewModel.Id);
            foreach (var item in driver.state)
                try
                {
                    var stateViewModel = new 
                    StateViewModel
                    {
                        Name = item.name, 
                        IsAdditional = true
                    };
                    if (
                        ViewModel.Current.SelectedDeviceViewModel.StatesViewModel.FirstOrDefault(
                            x => x.Id == stateViewModel.Id) == null)
                        Items.Add(stateViewModel);
                }catch{}
        }
    }
}