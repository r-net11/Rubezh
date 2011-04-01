using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace ServiceVisualizer
{
    public class EnumProperty : BaseViewModel
    {
        public string PropertyName { get; set; }

        string selectedValue;
        public string SelectedValue
        {
            get { return selectedValue; }
            set
            {
                selectedValue = value;
                OnPropertyChanged("SelectedValue");
            }
        }

        List<string> values;
        public List<string> Values
        {
            get { return values; }
            set
            {
                values = value;
                OnPropertyChanged("Values");
            }
        }
    }
}
