using Infrastructure.Common;

namespace JournalModule.ViewModels
{
    public class EventViewModel : BaseViewModel
    {
        public EventViewModel(string name)
        {
            Name = name;
        }

        string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        string _isEnabled;
        public string IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }
    }
}
