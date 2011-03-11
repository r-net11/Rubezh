using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Data
{
    [Serializable]
    public class DataSource
    {
        public List<DataItem> DataItems { get; set; }
    }

    [Serializable]
    public class DataItem : INotifyPropertyChanged
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

        string dataValue;
        public string DataValue
        {
            get { return dataValue; }
            set
            {
                dataValue = value;
                OnPropertyChanged("DataValue");
            }
        }

        public int Id { get; set; }

        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
