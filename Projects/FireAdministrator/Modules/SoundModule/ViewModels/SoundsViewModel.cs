using System.Collections.ObjectModel;
using Infrastructure.Common;
using System.Media;

namespace SoundsModule.ViewModels
{
    public class SoundsViewModel : RegionViewModel
    {
        public SoundsViewModel()
        {
            SoundPl = new SoundPlayer();
        }

        public void Initialize()
        {
            States = new ObservableCollection<StateViewModel>();

            States.Add(new StateViewModel("Тревога", SoundPl));
            States.Add(new StateViewModel("Внимание", SoundPl));
            States.Add(new StateViewModel("Неисправность", SoundPl));
            States.Add(new StateViewModel("Требуется обслуживание", SoundPl));
            States.Add(new StateViewModel("Отключено", SoundPl));
            States.Add(new StateViewModel("Неизвестно", SoundPl));
            States.Add(new StateViewModel("Норма(*)", SoundPl));
            States.Add(new StateViewModel("Норма", SoundPl));

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

        SoundPlayer _soundPl;
        public SoundPlayer SoundPl
        {
            get 
            {
                return _soundPl; 
            }

            set 
            {
                _soundPl = value;
            }
        }

    }
}
