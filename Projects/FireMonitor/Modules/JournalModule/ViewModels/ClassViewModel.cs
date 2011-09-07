using FiresecAPI.Models;
using Infrastructure.Common;

namespace JournalModule.ViewModels
{
    public class ClassViewModel : BaseViewModel
    {
        public ClassViewModel(StateType stateType)
        {
            Id = stateType;
        }

        public StateType Id { get; private set; }

        bool? _isEnable = false;
        public bool? IsEnable
        {
            get { return _isEnable; }
            set
            {
                _isEnable = value;
                OnPropertyChanged("IsEnable");
            }
        }
    }
}