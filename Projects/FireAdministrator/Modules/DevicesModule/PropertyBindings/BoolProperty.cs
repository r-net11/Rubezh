using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;

namespace DevicesModule.PropertyBindings
{
    public class BoolProperty : BaseViewModel
    {
        public string PropertyName { get; set; }

        bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }
    }
}
