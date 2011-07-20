using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common;

namespace LibraryModule.ViewModels
{
    public class AddAdditionalStateViewModel : DialogContent
    {
        public AddAdditionalStateViewModel(DeviceViewModel parentDevice)
        {
            ParentDevice = parentDevice;
            Initialize();
        }

        public void Initialize()
        {
            Title = "Добавить дополнительное состояние";

            States = new ObservableCollection<StateViewModel>();
            foreach (var state in ParentDevice.Driver.States)
            {
                if (ParentDevice.States.Any(x => x.Id == state.id && x.IsAdditional) == false)
                {
                    States.Add(new StateViewModel(state, ParentDevice));
                }
            }
            States = new ObservableCollection<StateViewModel>(
                 from state in States
                 orderby state.Name
                 orderby state.ClassName
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
