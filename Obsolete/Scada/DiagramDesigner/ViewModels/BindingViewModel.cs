using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Data;
using Common;
using DiagramDesigner.Views;

namespace DiagramDesigner.ViewModels
{
    public class BindingViewModel : INotifyPropertyChanged
    {
        public BindingView BindingView { get; set; }

        public RelayCommand SaveCommand { get; private set; }
        public BindingViewModel()
        {
            SaveCommand = new RelayCommand(OnSave);
        }

        void OnSave(object obj)
        {
            BindingView.Close();
        }

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

        DataItemViewModel selectedDataItem;
        public DataItemViewModel SelectedDataItem
        {
            get { return selectedDataItem; }
            set
            {
                selectedDataItem = value;
                OnPropertyChanged("SelectedDataItem");
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
