using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common;

namespace LibraryModule.ViewModels
{
    public class AddStateViewModel : DialogContent
    {
        public AddStateViewModel(DeviceViewModel parentDevice)
        {
            ParentDevice = parentDevice;
            Initialize();
        }

        void Initialize()
        {
            Title = "Добавить состояние";

            States = new ObservableCollection<StateViewModel>();
            for (int stateId = 0; stateId < 9; ++stateId)
            {
                string id = stateId.ToString();
                if (ParentDevice.States.Any(x => x.Id == id && !x.IsAdditional) == false)
                {
                    var stateViewModel = new StateViewModel(id, ParentDevice);
                    var defaultFrameViewModel = new FrameViewModel(stateViewModel);
                    stateViewModel.Frames = new ObservableCollection<FrameViewModel> { defaultFrameViewModel };

                    States.Add(stateViewModel);
                }
            }
            States = new ObservableCollection<StateViewModel>(
                from state in States
                orderby state.Name
                select state);

            OkCommand = new RelayCommand(OnOk);
            CancelCommand = new RelayCommand(OnCancel);
        }

        DeviceViewModel ParentDevice { get; set; }

        ObservableCollection<StateViewModel> _states;
        public ObservableCollection<StateViewModel> States
        {
            get
            {
                return _states;
            }

            set
            {
                _states = value;
                OnPropertyChanged("States");
            }
        }

        StateViewModel _selectedState;
        public StateViewModel SelectedState
        {
            get
            {
                return _selectedState;
            }

            set
            {
                _selectedState = value;
                OnPropertyChanged("SelectedState");
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
