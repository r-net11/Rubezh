using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssadProcessor;
using System.Diagnostics;
using System.ComponentModel;
using Infrastructure.Common;

namespace AutoHosting
{
    public class ViewModel : BaseViewModel
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
        void OnStart()
        {
            controller = new Controller();
            controller.Start();
            Status = "Running";
            MessageProcessor.NewMessage += new Action<string>(MessageProcessor_NewMessage);
        }

        int commandCount = 0;
        void MessageProcessor_NewMessage(string message)
        {
            LastCommand = (commandCount++).ToString() + " - " + message;
        }

        public RelayCommand StopCommand { get; private set; }
        void OnStop()
        {
            if (controller != null)
            {
                controller.Stop();
                controller = null;
            }
            Status = "Stopped";
        }

        public RelayCommand TestCommand { get; private set; }
        void OnTest()
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

        string lastCommand;
        public string LastCommand
        {
            get { return lastCommand; }
            set
            {
                lastCommand = value;
                OnPropertyChanged("LastCommand");
            }
        }
    }
}
