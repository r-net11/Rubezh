using System;
using AssadProcessor;
using Infrastructure.Common;

namespace AutoHosting
{
    public class ViewModel : BaseViewModel
    {
        Controller _controller;

        public ViewModel()
        {
            StartCommand = new RelayCommand(OnStart);
            StopCommand = new RelayCommand(OnStop);
            TestCommand = new RelayCommand(OnTest);
        }

        int _commandCount = 0;
        void MessageProcessor_NewMessage(string message)
        {
            LastCommand = (_commandCount++).ToString() + " - " + message;
        }

        string _status = "None";
        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }

        string _lastCommand;
        public string LastCommand
        {
            get { return _lastCommand; }
            set
            {
                _lastCommand = value;
                OnPropertyChanged("LastCommand");
            }
        }

        public RelayCommand StartCommand { get; private set; }
        void OnStart()
        {
            _controller = new Controller();
            _controller.Start();
            Status = "Running";
            MessageProcessor.NewMessage += new Action<string>(MessageProcessor_NewMessage);
        }

        public RelayCommand StopCommand { get; private set; }
        void OnStop()
        {
            if (_controller != null)
            {
                _controller.Stop();
                _controller = null;
            }
            Status = "Stopped";
        }

        public RelayCommand TestCommand { get; private set; }
        void OnTest()
        {
        }
    }
}