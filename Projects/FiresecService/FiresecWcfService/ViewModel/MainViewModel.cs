using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using FiresecWcfService.Service;
using System.ComponentModel;

namespace FiresecWcfService.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
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
            FiresecServiceManager.Open();
            Status = "Running";
        }

        public RelayCommand StopCommand { get; private set; }
        void OnStop(object parameter)
        {
            FiresecServiceManager.Close();
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
    }
}
