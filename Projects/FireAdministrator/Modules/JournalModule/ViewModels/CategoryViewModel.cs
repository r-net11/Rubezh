using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;

namespace JournalModule.ViewModels
{
    public class CategoryViewModel : BaseViewModel
    {
        public CategoryViewModel(string name)
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
