using FiresecAPI.Models;
using Infrastructure.Common;

namespace JournalModule.ViewModels
{
    public class EventViewModel : BaseViewModel
    {
        public EventViewModel(StateType stateType, string name)
        {
            ClassId = (int) stateType;
            Name = name;
        }

        public int ClassId { get; private set; }
        public string Name { get; private set; }

        bool _isEnable;
        public bool IsEnable
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