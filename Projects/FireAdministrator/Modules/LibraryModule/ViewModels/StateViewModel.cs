using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common;
using System.Windows;
using DeviceLibrary;
using System.Collections.ObjectModel;
using DeviceLibrary.Models;

namespace LibraryModule.ViewModels
{
    public class StateViewModel : BaseViewModel
    {
        public StateViewModel()
        {
            Current = this;
            ParentDevice = DeviceViewModel.Current;
            RemoveStateCommand = new RelayCommand(OnRemoveState);
            ShowStatesCommand = ParentDevice.ShowStatesCommand;
            Frames = new ObservableCollection<FrameViewModel>();
        }

        public void Initialize(State state)
        {
            IsAdditional = state.IsAdditional;
            Id = state.Id;            
        }

        public static StateViewModel Current { get; private set; }

        public DeviceViewModel ParentDevice { get; set; }

        private FrameViewModel _selectedFrame;
        public FrameViewModel SelectedFrame
        {
            get { return _selectedFrame; }
            set
            {
                _selectedFrame = value;
                OnPropertyChanged("SelectedFrame");
            }
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                if (LibraryViewModel.Current.SelectedState == null)
                    LibraryViewModel.Current.SelectedState = this;
                if (this.ParentDevice != LibraryViewModel.Current.SelectedState.ParentDevice)
                    LibraryViewModel.Current.SelectedState = this;
                if (_isChecked)
                {
                    LibraryViewModel.Current.SelectedState.ParentDevice.AdditionalStates.Add(Id);
                }
                if (!_isChecked)
                {
                    LibraryViewModel.Current.SelectedState.ParentDevice.AdditionalStates.Remove(Id);
                }
                if (LibraryViewModel.Current.SelectedState.IsAdditional) return;

                var tempAstate = new List<string>();
                foreach (var stateId in LibraryViewModel.Current.SelectedState.ParentDevice.AdditionalStates)
                {
                    var state = ParentDevice.States.FirstOrDefault(x => (x.Id == stateId)&&(x.IsAdditional));
                    if (state.Class(LibraryViewModel.Current.SelectedDevice) == LibraryViewModel.Current.SelectedState.Id)
                        tempAstate.Add(state.Id);
                }
                LibraryViewModel.Current.SelectedState.ParentDevice.DeviceControl.AdditionalStates = tempAstate;

                OnPropertyChanged("IsChecked");
            }
        }

        public string Class(DeviceViewModel device)
        {
            var astate = LibraryManager.Drivers.FirstOrDefault(x => x.id == device.Id).state.FirstOrDefault(x => x.id == this.Id);
            return astate.@class;
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

        private ObservableCollection<FrameViewModel> _frames;
        public ObservableCollection<FrameViewModel> Frames
        {
            get { return _frames; }
            set
            {
                _frames = value;
                OnPropertyChanged("Frames");
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

        public RelayCommand ShowStatesCommand { get; private set; }

        public RelayCommand RemoveStateCommand { get; private set; }
        private void OnRemoveState()
        {
            if (!IsAdditional)
            {
                MessageBox.Show("Невозможно удалить основное состояние");
                return;
            }
            IsChecked = false;
            ParentDevice.States.Remove(this);
            LibraryViewModel.Current.SelectedState = null;
        }
    }
}
