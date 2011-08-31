using FiresecAPI.Models;
using Infrastructure.Common;

namespace SecurityModule.ViewModels
{
    public class NewUserViewModel : SaveCancelDialogContent
    {
        public NewUserViewModel(User newUser)
        {
            Title = "Новый пользователь";
            _user = newUser;
        }

        User _user;

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
        public string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged("Password");
            }
        }

        protected override void Save(ref bool cancel)
        {
            _user.Name = Name;
            _user.FullName = FullName;
        }
    }
}