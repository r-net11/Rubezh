using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace WpfApplication4
{
    public class MainAlarm : BaseViewModel
    {
        string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        int count;
        public int Count
        {
            get { return count; }
            set
            {
                count = value;
                OnPropertyChanged("Count");
            }
        }
    }
}
