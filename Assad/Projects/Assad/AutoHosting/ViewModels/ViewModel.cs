using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using AssadProcessor;
using System.Diagnostics;
using System.ComponentModel;

namespace AutoHosting
{
    public class ViewModel : INotifyPropertyChanged
    {
        Controller controller;

        public ViewModel()
        {
            StartCommand = new RelayCommand(OnStart);
            StopCommand = new RelayCommand(OnStop);
            TestCommand = new RelayCommand(OnTest);
            Status = "None";
        }

        public RelayCommand StartCommand { get; private set; }
        void OnStart(object parameter)
        {
            controller = new Controller();
            controller.Start();
            Status = "Running";
        }

        public RelayCommand StopCommand { get; private set; }
        void OnStop(object parameter)
        {
            if (controller != null)
            {
                controller.Stop();
                controller = null;
            }
            Status = "Stopped";
        }

        public RelayCommand TestCommand { get; private set; }
        void OnTest(object parameter)
        {
        }

        string status;
        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged("Status");
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
