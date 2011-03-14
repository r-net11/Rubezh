using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using ServiseProcessor;
using System.ComponentModel;

namespace WCFService.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        Processor processor;

        public MainViewModel()
        {
            StartCommand = new RelayCommand(OnStart);
            StopCommand = new RelayCommand(OnStop);
            TestCommand = new RelayCommand(OnTest);
            Status = "None";
        }

        public RelayCommand StartCommand { get; private set; }
        void OnStart(object parameter)
        {
            processor = new Processor();
            processor.Start();
            Status = "Running";
        }

        public RelayCommand StopCommand { get; private set; }
        void OnStop(object parameter)
        {
            if (processor != null)
            {
                processor.Stop();
                processor = null;
            }
            Status = "Stopped";
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

        public RelayCommand TestCommand { get; private set; }
        void OnTest(object parameter)
        {
        }

        ~MainViewModel()
        {
            OnStop(null);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
