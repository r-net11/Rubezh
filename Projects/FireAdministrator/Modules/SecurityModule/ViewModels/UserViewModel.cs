using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using Firesec.CoreConfig;

namespace SecurityModule.ViewModels
{
    public class UserViewModel : BaseViewModel
    {
        public UserViewModel(userType user)
        {
            Name = user.name;
            FullName = user.fullName;
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
