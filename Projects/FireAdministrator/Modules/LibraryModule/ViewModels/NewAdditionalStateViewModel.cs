using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common;

namespace LibraryModule.ViewModels
{
    internal class NewAdditionalStateViewModel : DialogContent
    {
        public NewAdditionalStateViewModel()
        {
            _selectedDevice = LibraryViewModel.Current.SelectedDevice;
            Initialize();
        }

        public void Initialize()
        {
            Title = "Добавить дополнительное состояние";
            States = new ObservableCollection<StateViewModel>();

            var driver = FiresecManager.Configuration.Drivers.FirstOrDefault(x => x.Id == _selectedDevice.Id);
            foreach (var innerState in driver.States)
            {
                if (_selectedDevice.States.Any(x => x.Id == innerState.id && x.IsAdditional) == false &&
                    _selectedDevice.States.Any(x => x.IsAdditional == false && x.Id.Equals(innerState.@class)) == true)
                {
                    var stateViewModel = new StateViewModel(innerState.id, _selectedDevice, true);
                    var defaultFrameViewModel = new FrameViewModel(Helper.EmptyFrame, 300, 0);
                    stateViewModel.Frames = new ObservableCollection<FrameViewModel> { defaultFrameViewModel };
                    States.Add(stateViewModel);
                }
            }
            States = new ObservableCollection<StateViewModel>(
                 from state in States
                 orderby state.ClassName
                 select state);

            AddCommand = new RelayCommand(OnAdd);
            CancelCommand = new RelayCommand(OnCancel);
        }

        private readonly DeviceViewModel _selectedDevice;

        private StateViewModel _selectedState;
        public StateViewModel SelectedState
        {
            get { return _selectedState; }
            set
            {
                _selectedState = value;
                if (value == null)
                {
                    return;
                }

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

        public RelayCommand AddCommand { get; private set; }
        private void OnAdd()
        {
            if (SelectedState == null) return;
            _selectedDevice.States.Add(SelectedState);
            //States.Remove(SelectedState);
            LibraryViewModel.Current.Update();
            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        private void OnCancel()
        {
            Close(false);
        }
    }
}
