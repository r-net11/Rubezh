using FiresecAPI.Models;
using Infrastructure.Common;

namespace JournalModule.ViewModels
{
    public class SubsystemViewModel : BaseViewModel
    {
        public SubsystemViewModel(SubsystemType subsystem)
        {
            Subsystem = subsystem;
        }

        public SubsystemType Subsystem { get; private set; }

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