using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Windows;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Threading;
using System.Threading;

namespace AssadEmulator
{
    class AssadEmulatorViewModel : INotifyPropertyChanged
    {
        public AssadEmulatorViewModel()
        {
            ConnectCommand = new RelayCommand(x => SocketServer.Connect(), x => !SocketServer.Connected);
            EmulateAllCommand = new RelayCommand(OnEmulateAll);

            Messages = new ObservableCollection<Message>();
            Messages.Add(new Message("MHqueryCP"));
            Messages.Add(new Message("MHqueryConfiguredDev"));
            Messages.Add(new Message("MHconfig"));
            Messages.Add(new Message("MHqueryState"));
            Messages.Add(new Message("MHping"));

            SocketServer.Recieved += (string message) => { RecievedMessage += message + "\n"; };
        }

        string recievedMessage;
        public string RecievedMessage
        {
            get { return recievedMessage; }
            set
            {
                recievedMessage = value;
                OnPropertyChanged("RecievedMessage");
            }
        }

        public ObservableCollection<Message> Messages { get; set; }

        public RelayCommand ConnectCommand { get; private set; }
        public RelayCommand EmulateAllCommand { get; private set; }

        void OnEmulateAll(object parameters)
        {
            ConnectCommand.Execute(null);
            Thread.Sleep(100);
            foreach(Message message in Messages)
            {
                message.SendCommand.Execute(null);
                Thread.Sleep(200);
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
