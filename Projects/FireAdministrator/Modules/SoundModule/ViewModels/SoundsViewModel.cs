using System.Collections.ObjectModel;
using Infrastructure.Common;

namespace SoundsModule.ViewModels
{
    public class SoundsViewModel : RegionViewModel
    {
        public SoundsViewModel()
        {
        }

        public void Initialize()
        {
            States = new ObservableCollection<StateViewModel>();

            States.Add(new StateViewModel("Тревога"));
            States.Add(new StateViewModel("Внимание"));
            States.Add(new StateViewModel("Неисправность"));
            States.Add(new StateViewModel("Требуется обслуживание"));
            States.Add(new StateViewModel("Отключено"));
            States.Add(new StateViewModel("Неизвестно"));
            States.Add(new StateViewModel("Норма(*)"));
            States.Add(new StateViewModel("Норма"));

            SelectedState = States[0];
        }

        ObservableCollection<StateViewModel> _states;
        public ObservableCollection<StateViewModel> States
        {
            get { return _states; }
            set
            {
                _states = value;
                OnPropertyChanged("States");
            }
        }

        StateViewModel _selectedState;
        public StateViewModel SelectedState
        {
            get { return _selectedState; }
            set
            {
                _selectedState = value;
                OnPropertyChanged("SelectedState");
            }
        }
    }
}
