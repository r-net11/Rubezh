using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DeviceEditor
{
    public class StateViewModel : INotifyPropertyChanged
    {
        public string id;
        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }

        public ObservableCollection<CadrViewModel> cadrViewModels;
        public ObservableCollection<CadrViewModel> CadrViewModels
        {
            get { return cadrViewModels; }
            set
            {
                cadrViewModels = value;
                OnPropertyChanged("CadrViewModels");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
