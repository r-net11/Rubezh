using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Runner.ViewModels
{
    public class DataItemViewModel : INotifyPropertyChanged
    {
        Data.DataItem dataItem;
        public Data.DataItem DataItem
        {
            get { return dataItem; }
            set
            {
                dataItem = value;
                OnPropertyChanged("DataItem");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
