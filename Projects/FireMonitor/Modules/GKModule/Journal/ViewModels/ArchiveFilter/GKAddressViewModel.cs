using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
    public class GKAddressViewModel : BaseViewModel
    {
        public GKAddressViewModel(string address)
        {
            Address = address;
        }

        public string Address { get; private set; }

        bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }
    }
}
