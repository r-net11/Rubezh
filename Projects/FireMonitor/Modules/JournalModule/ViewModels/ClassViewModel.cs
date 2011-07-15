using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;

namespace JournalModule.ViewModels
{
    public class ClassViewModel : BaseViewModel
    {
        public ClassViewModel(string name)
        {
            _name = name;
        }

        string _name;
        public string Name
        {
            get { return _name; }
        }

        bool _isEnabled;
        public bool IsEnabled
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
