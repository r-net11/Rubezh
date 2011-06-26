using System;
using System.Linq;
using DeviceLibrary;
using Firesec.Metadata;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using FiresecClient;

namespace LibraryModule.ViewModels
{
    class NewStateViewModel : DialogContent
    {
        public NewStateViewModel()
        {
            Title = "Добавить состояние";
            _selectedDevice = LibraryViewModel.Current.SelectedDevice;
            Initialize();
            AddCommand = new RelayCommand(OnAdd);
            CancelCommand = new RelayCommand(OnCancel);
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }

        private readonly DeviceViewModel _selectedDevice;

        private StateViewModel _selectedState;
        public StateViewModel SelectedState
        {
            get { return _selectedState; }
            set
            {
                _selectedState = value;
                IsEnabled = true;
                OnPropertyChanged("SelectedState");
            }
        }

        private ObservableCollection<StateViewModel> _states;
        public ObservableCollection<StateViewModel> States
        {
            get { return _states; }
            set
            {
                _states = value;
                OnPropertyChanged("States");
            }
        }

        public void Initialize()
        {
            States = new ObservableCollection<StateViewModel>();
            for (var stateId = 0; stateId < 9; stateId++)
            {
                var driver = FiresecManager.Configuration.Drivers.FirstOrDefault(x => x.Id == _selectedDevice.Id);

                if (_selectedDevice.States.FirstOrDefault(x => (x.Id == Convert.ToString(stateId)) && (!x.IsAdditional)) != null)
                    continue;
                if (stateId != 7)
                    if (driver.States.FirstOrDefault(x => x.@class == Convert.ToString(stateId)) == null)
                        continue;
                var stateViewModel = new StateViewModel(Convert.ToString(stateId), _selectedDevice, false);
                var frames = new ObservableCollection<FrameViewModel> { new FrameViewModel(Helper.EmptyFrame, 300, 0) };
                stateViewModel.Frames = frames;
                States.Add(stateViewModel);
            }
        }

        public RelayCommand AddCommand { get; private set; }
        private void OnAdd()
        {
            if (SelectedState == null) return;
            _selectedDevice.States.Add(SelectedState);
            _selectedDevice.SortStates();
            LibraryViewModel.Current.Update();
            IsEnabled = false;
            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        private void OnCancel()
        {
            Close(false);
        }
    }
}
