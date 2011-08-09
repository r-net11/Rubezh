using FiresecAPI.Models;
using Infrastructure.Common;

namespace SecurityModule.ViewModels
{
    public class UserViewModel : BaseViewModel
    {
        public UserViewModel(User user)
        {
            Name = user.Name;
            FullName = user.FullName;
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

        string _fullName;
        public string FullName
        {
            get { return _fullName; }
            set
            {
                _fullName = value;
                OnPropertyChanged("FullName");
            }
        }
    }
}