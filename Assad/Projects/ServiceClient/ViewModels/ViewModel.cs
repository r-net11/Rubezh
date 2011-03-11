using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using ServiceClient.StateServiceReference;
using System.Collections.ObjectModel;
using Common;

namespace ServiceClient
{
    public class ViewModel : INotifyPropertyChanged
    {
        public Controller controller { get; set; }

        public ViewModel()
        {
            Start = new RelayCommand(OnStart);
        }

        public RelayCommand Start {get; private set;}

        public void OnStart(object parameter)
        {
            controller.Start();
        }

        string messages;
        public string Messages
        {
            get { return messages; }
            set
            {
                messages = value;
                OnPropertyChanged("Messages");
            }
        }

        ObservableCollection<ComDevice> comDevices;
        public ObservableCollection<ComDevice> ComDevices
        {
            get { return comDevices; }
            set
            {
                comDevices = value;
                OnPropertyChanged("ComDevices");
            }
        }

        ComDevice selectedComDevice;
        public ComDevice SelectedComDevice
        {
            get { return selectedComDevice; }
            set
            {
                selectedComDevice = value;
                OnPropertyChanged("SelectedComDevice");
            }
        }

        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
