using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common;

namespace LibraryModule.ViewModels
{
    public class NewStateViewModel : DialogContent
    {
        public NewStateViewModel()
        {
            _selectedDevice = LibraryViewModel.Current.SelectedDevice;
            Initialize();
        }

        void Initialize()
        {
            Title = "Добавить состояние";

            States = new ObservableCollection<StateViewModel>();
            var driver = FiresecManager.Configuration.Drivers.FirstOrDefault(x => x.Id == _selectedDevice.Id);
            for (int stateId = 0; stateId < 9; ++stateId)
            {
                string id = stateId.ToString();
                if (_selectedDevice.States.Any(x => x.Id == id && !x.IsAdditional) == false)
                {
                    var stateViewModel = new StateViewModel(id, _selectedDevice, false);
                    var defaultFrameViewModel = new FrameViewModel(Helper.EmptyFrame, 300, 0);
                    stateViewModel.Frames = new ObservableCollection<FrameViewModel> { defaultFrameViewModel };

                    States.Add(stateViewModel);
                }
            }

            AddCommand = new RelayCommand(OnAdd);
            CancelCommand = new RelayCommand(OnCancel);
        }

        private readonly DeviceViewModel _selectedDevice;
                
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

        private StateViewModel _selectedState;
        public StateViewModel SelectedState
        {
            get { return _selectedState; }
            set
            {
                _selectedState = value;
                OnPropertyChanged("SelectedState");
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
