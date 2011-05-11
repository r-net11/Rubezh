using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Common;
using DeviceLibrary.Models;

namespace DeviceEditor.ViewModels
{
    public class StateViewModel : BaseViewModel
    {
        public StateViewModel()
        {
            Current = this;
            ParentDevice = DeviceViewModel.Current;
            RemoveStateCommand = new RelayCommand(OnRemoveState);
        }

        public static StateViewModel Current { get; private set; }

        public DeviceViewModel ParentDevice { get; set; }

        private FrameViewModel _selectedFrameViewModel;
        public FrameViewModel SelectedFrameViewModel
        {
            get { return _selectedFrameViewModel; }
            set
            {
                _selectedFrameViewModel = value;
                OnPropertyChanged("SelectedFrameViewModel");
            }
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                if (_isChecked)
                {
                    ViewModel.Current.SelectedStateViewModel.ParentDevice.AdditionalStatesViewModel.Add(Id);
                }
                if (!_isChecked)
                {
                    ViewModel.Current.SelectedStateViewModel.ParentDevice.AdditionalStatesViewModel.Remove(Id);
                }
                ViewModel.Current.SelectedStateViewModel.ParentDevice.DeviceControl.AdditionalStatesIds =
                    ViewModel.Current.SelectedStateViewModel.ParentDevice.AdditionalStatesViewModel;
                OnPropertyChanged("IsChecked");
            }
        }

        private string _iconPath = Helper.StatesIconPath;
        public string IconPath
        {
            get { return _iconPath; }
            set
            {
                _iconPath = value;
                OnPropertyChanged("IconPath");
            }
        }

        public RelayCommand RemoveStateCommand { get; private set; }
        private void OnRemoveState(object obj)
        {
            if (!IsAdditional)
            {
                MessageBox.Show("Невозможно удалить основное состояние");
                return;
            }
            IsChecked = false;
            ParentDevice.StatesViewModel.Remove(this);
            ViewModel.Current.SelectedStateViewModel = null;
        }

        private string _id;
        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                var driver = LibraryManager.Drivers.FirstOrDefault(x => x.id == ParentDevice.Id);
                Name = IsAdditional ? driver.state.FirstOrDefault(x => x.id == _id).name : Helper.BaseStatesList[Convert.ToInt16(_id)];
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private ObservableCollection<FrameViewModel> _frameViewModels;
        public ObservableCollection<FrameViewModel> FrameViewModels
        {
            get { return _frameViewModels; }
            set
            {
                _frameViewModels = value;
                OnPropertyChanged("FrameViewModels");
            }
        }

        private bool _isAdditional;
        public bool IsAdditional
        {
            get { return _isAdditional; }
            set
            {
                _isAdditional = value;
                OnPropertyChanged("IsAdditional");
            }
        }
    }
}