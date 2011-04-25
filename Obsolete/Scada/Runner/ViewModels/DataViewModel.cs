using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Data;
using System.Collections.ObjectModel;

namespace Runner.ViewModels
{
    public class DataViewModel : INotifyPropertyChanged
    {
        public void Initialize(List<DataItem> DataItemList)
        {
            MyDataItems = new ObservableCollection<DataItemViewModel>();
            foreach (DataItem dataItem in DataItemList)
            {
                DataItemViewModel dataItemViewModel = new DataItemViewModel();
                dataItemViewModel.DataItem = dataItem;
                MyDataItems.Add(dataItemViewModel);
            }
        }

        ObservableCollection<DataItemViewModel> dataItems;
        public ObservableCollection<DataItemViewModel> MyDataItems
        {
            get { return dataItems; }
            set
            {
                dataItems = value;
                OnPropertyChanged("MyDataItems");
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
